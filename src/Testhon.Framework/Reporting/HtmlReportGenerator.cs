using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Testhon.Framework.Reporting;

/// <summary>
/// Renders <see cref="RunReport"/> into a single self-contained HTML file (all CSS/JS inlined,
/// no external dependencies) with a summary dashboard, searchable/filterable test cards, step
/// timelines, embedded failure screenshots and artifact links. Works fully offline.
/// </summary>
public static class HtmlReportGenerator
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static string Generate(string outputPath)
    {
        RunReport.Stop();

        var tests = RunReport.Tests;
        var reportDir = Path.GetDirectoryName(Path.GetFullPath(outputPath)) ?? AppContext.BaseDirectory;

        var total = tests.Count;
        var passed = tests.Count(t => t.Outcome == TestOutcome.Passed);
        var failed = tests.Count(t => t.Outcome == TestOutcome.Failed);
        var skipped = tests.Count(t => t.Outcome == TestOutcome.Skipped);
        var passRate = total == 0 ? 0d : passed * 100d / total;
        var wallClock = RunReport.EndTime - RunReport.StartTime;

        var passPct = total == 0 ? 0d : passed * 100d / total;
        var failEndPct = total == 0 ? 0d : (passed + failed) * 100d / total;

        var html = new StringBuilder();
        html.Append("<!DOCTYPE html><html lang=\"en\" data-theme=\"dark\"><head>");
        html.Append("<meta charset=\"utf-8\"/>");
        html.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/>");
        html.Append("<title>Testhon Test Report</title>");
        html.Append("<style>").Append(Css).Append("</style>");
        html.Append("</head><body>");

        // ---- Top bar ----
        html.Append("<header class=\"topbar\">");
        html.Append("<div class=\"brand\"><span class=\"logo\">T</span><div><div class=\"brand-title\">Testhon Test Report</div>")
            .Append("<div class=\"brand-sub\">Generated ").Append(Enc(DateTimeOffset.Now.ToString("dddd, dd MMM yyyy HH:mm:ss", Inv))).Append("</div></div></div>");
        html.Append("<button id=\"themeToggle\" class=\"icon-btn\" title=\"Toggle light / dark\" aria-label=\"Toggle theme\">◐</button>");
        html.Append("</header>");

        html.Append("<main class=\"container\">");

        // ---- Summary dashboard ----
        html.Append("<section class=\"dashboard\">");

        // Donut
        var donutStyle = string.Create(Inv,
            $"background:conic-gradient(var(--pass) 0 {passPct:0.##}%, var(--fail) {passPct:0.##}% {failEndPct:0.##}%, var(--skip) {failEndPct:0.##}% 100%);");
        html.Append("<div class=\"donut-card\">");
        html.Append("<div class=\"donut\" style=\"").Append(donutStyle).Append("\">");
        html.Append("<div class=\"donut-hole\"><div class=\"donut-rate\">").Append(passRate.ToString("0.#", Inv)).Append("%</div><div class=\"donut-label\">passed</div></div>");
        html.Append("</div>");
        html.Append("<div class=\"legend\">");
        html.Append(Legend("pass", "Passed", passed));
        html.Append(Legend("fail", "Failed", failed));
        html.Append(Legend("skip", "Skipped", skipped));
        html.Append("</div></div>");

        // KPI cards (Total/Passed/Failed/Skipped double as filters that jump to the test list)
        html.Append("<div class=\"kpis\">");
        html.Append(Kpi("Total", total.ToString(Inv), "total", "all"));
        html.Append(Kpi("Passed", passed.ToString(Inv), "pass", "passed"));
        html.Append(Kpi("Failed", failed.ToString(Inv), "fail", "failed"));
        html.Append(Kpi("Skipped", skipped.ToString(Inv), "skip", "skipped"));
        html.Append(Kpi("Pass rate", passRate.ToString("0.#", Inv) + "%", "rate"));
        html.Append(Kpi("Duration", FormatDuration(wallClock), "time"));
        html.Append("</div>");

        html.Append("</section>");

        // ---- Environment ----
        var sysInfo = RunReport.SystemInfo;
        if (sysInfo.Count > 0)
        {
            html.Append("<section class=\"panel env\"><h2>Environment</h2><div class=\"env-grid\">");
            foreach (var kv in sysInfo.OrderBy(k => k.Key))
            {
                html.Append("<div class=\"env-item\"><span class=\"env-key\">").Append(Enc(kv.Key))
                    .Append("</span><span class=\"env-val\">").Append(Enc(kv.Value)).Append("</span></div>");
            }
            html.Append("</div></section>");
        }

        // ---- Toolbar ----
        html.Append("<section class=\"toolbar\">");
        html.Append("<div class=\"search\"><input id=\"search\" type=\"search\" placeholder=\"Search tests…\" autocomplete=\"off\"/></div>");
        html.Append("<div class=\"filters\">");
        html.Append("<button class=\"chip active\" data-filter=\"all\">All <b>").Append(total.ToString(Inv)).Append("</b></button>");
        html.Append("<button class=\"chip\" data-filter=\"passed\">Passed <b>").Append(passed.ToString(Inv)).Append("</b></button>");
        html.Append("<button class=\"chip\" data-filter=\"failed\">Failed <b>").Append(failed.ToString(Inv)).Append("</b></button>");
        html.Append("<button class=\"chip\" data-filter=\"skipped\">Skipped <b>").Append(skipped.ToString(Inv)).Append("</b></button>");
        html.Append("</div>");
        html.Append("<div class=\"expando\"><button id=\"expandAll\" class=\"ghost-btn\">Expand all</button><button id=\"collapseAll\" class=\"ghost-btn\">Collapse all</button></div>");
        html.Append("</section>");

        // ---- Test list ----
        html.Append("<section id=\"tests\" class=\"tests\">");
        if (total == 0)
        {
            html.Append("<div class=\"empty\">No tests were recorded in this run.</div>");
        }
        foreach (var t in tests)
        {
            html.Append(RenderTest(t, reportDir));
        }
        html.Append("</section>");

        html.Append("</main>");

        // ---- Footer ----
        html.Append("<footer class=\"footer\">Testhon automation framework · .NET ")
            .Append(Enc(Environment.Version.ToString()))
            .Append(" · ").Append(Enc(RuntimeInformation.OSDescription.Trim()))
            .Append("</footer>");

        // ---- Lightbox + script ----
        html.Append("<div id=\"lightbox\" class=\"lightbox\"><img id=\"lightbox-img\" alt=\"screenshot\"/></div>");
        html.Append("<script>").Append(Script).Append("</script>");
        html.Append("</body></html>");

        Directory.CreateDirectory(reportDir);
        File.WriteAllText(outputPath, html.ToString(), Encoding.UTF8);
        return outputPath;
    }

    private static string RenderTest(TestRecord t, string reportDir)
    {
        var status = t.Outcome switch
        {
            TestOutcome.Passed => "passed",
            TestOutcome.Failed => "failed",
            _ => "skipped"
        };
        var openClass = t.Outcome == TestOutcome.Failed ? " open" : string.Empty;

        var sb = new StringBuilder();
        sb.Append("<article class=\"test ").Append(status).Append(openClass).Append("\" data-status=\"").Append(status)
          .Append("\" data-name=\"").Append(Enc((t.Name + " " + (t.ClassName ?? string.Empty)).ToLowerInvariant())).Append("\">");

        // Header (clickable)
        sb.Append("<button class=\"test-head\" type=\"button\">");
        sb.Append("<span class=\"pill ").Append(status).Append("\">").Append(status.ToUpperInvariant()).Append("</span>");
        sb.Append("<span class=\"test-name\">").Append(Enc(t.Name));
        if (!string.IsNullOrWhiteSpace(t.ClassName))
        {
            sb.Append("<span class=\"test-class\">").Append(Enc(ShortClass(t.ClassName!))).Append("</span>");
        }
        sb.Append("</span>");
        sb.Append("<span class=\"test-meta\">");
        if (t.Categories.Count > 0)
        {
            foreach (var c in t.Categories)
            {
                sb.Append("<span class=\"tag\">").Append(Enc(c)).Append("</span>");
            }
        }
        sb.Append("<span class=\"dur\">").Append(FormatDuration(t.Duration)).Append("</span>");
        sb.Append("<span class=\"chevron\">▾</span>");
        sb.Append("</span>");
        sb.Append("</button>");

        // Body
        sb.Append("<div class=\"test-body\">");

        if (!string.IsNullOrWhiteSpace(t.Description))
        {
            sb.Append("<p class=\"desc\">").Append(Enc(t.Description!)).Append("</p>");
        }

        sb.Append("<div class=\"timing\">Started ").Append(Enc(t.StartTime.ToString("HH:mm:ss.fff", Inv)))
          .Append(" · Ended ").Append(Enc(t.EndTime.ToString("HH:mm:ss.fff", Inv))).Append("</div>");

        // Error block
        if (t.Outcome == TestOutcome.Failed && (!string.IsNullOrWhiteSpace(t.ErrorMessage) || !string.IsNullOrWhiteSpace(t.StackTrace)))
        {
            sb.Append("<div class=\"error\">");
            if (!string.IsNullOrWhiteSpace(t.ErrorMessage))
            {
                sb.Append("<div class=\"error-msg\">").Append(Enc(t.ErrorMessage!)).Append("</div>");
            }
            if (!string.IsNullOrWhiteSpace(t.StackTrace))
            {
                sb.Append("<pre class=\"stack\">").Append(Enc(t.StackTrace!)).Append("</pre>");
            }
            sb.Append("</div>");
        }

        // Steps table (which method step passed / where it failed)
        var steps = t.Steps;
        if (steps.Count > 0)
        {
            var statuses = ComputeStepStatuses(steps, t.Outcome, t.BodyStepCount);
            sb.Append("<div class=\"steps-head\">Steps <span class=\"count\">").Append(steps.Count.ToString(Inv)).Append("</span></div>");
            sb.Append("<div class=\"step-table-wrap\"><table class=\"step-table\"><thead><tr>")
              .Append("<th class=\"step-num\">#</th><th>Step</th><th class=\"step-col-status\">Status</th>")
              .Append("<th class=\"step-col-time\">Time</th><th class=\"step-col-dur\">Elapsed</th></tr></thead><tbody>");
            for (var i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                var (label, cls) = statuses[i];
                var rowCls = cls == "fail" ? " class=\"row-fail\"" : string.Empty;
                var delta = i == 0 ? s.Timestamp - t.StartTime : s.Timestamp - steps[i - 1].Timestamp;
                sb.Append("<tr").Append(rowCls).Append(">")
                  .Append("<td class=\"step-num\">").Append((i + 1).ToString(Inv)).Append("</td>")
                  .Append("<td class=\"step-msg-cell\">").Append(Enc(s.Message)).Append("</td>")
                  .Append("<td><span class=\"st st-").Append(cls).Append("\">").Append(label).Append("</span></td>")
                  .Append("<td class=\"step-col-time\">").Append(Enc(s.Timestamp.ToString("HH:mm:ss.fff", Inv))).Append("</td>")
                  .Append("<td class=\"step-col-dur\">").Append(FormatDuration(delta)).Append("</td>")
                  .Append("</tr>");
            }
            sb.Append("</tbody></table></div>");
        }

        // Artifacts
        var hasArtifacts = t.ScreenshotBase64 is not null || t.TracePath is not null || t.VideoPath is not null;
        if (hasArtifacts)
        {
            sb.Append("<div class=\"artifacts\">");

            if (t.ScreenshotBase64 is not null)
            {
                sb.Append("<div class=\"art-block\"><div class=\"art-label\">Screenshot</div>")
                  .Append("<img class=\"shot\" src=\"data:image/png;base64,").Append(t.ScreenshotBase64)
                  .Append("\" alt=\"failure screenshot\" loading=\"lazy\"/></div>");
            }

            if (t.TracePath is not null)
            {
                sb.Append(ArtifactLink("Trace", t.TracePath, reportDir, "Open with: playwright show-trace"));
            }
            if (t.VideoPath is not null)
            {
                sb.Append(ArtifactLink("Video", t.VideoPath, reportDir, null));
            }

            sb.Append("</div>");
        }

        sb.Append("</div></article>");
        return sb.ToString();
    }

    private static string ArtifactLink(string label, string absolutePath, string reportDir, string? hint)
    {
        string href;
        try
        {
            href = Path.GetRelativePath(reportDir, absolutePath).Replace('\\', '/');
        }
        catch
        {
            href = absolutePath.Replace('\\', '/');
        }

        var sb = new StringBuilder();
        sb.Append("<div class=\"art-block\"><div class=\"art-label\">").Append(Enc(label)).Append("</div>");
        var encodedHref = string.Join('/', href.Split('/').Select(Uri.EscapeDataString));
        sb.Append("<a class=\"art-link\" href=\"").Append(Enc(encodedHref)).Append("\">").Append(Enc(Path.GetFileName(absolutePath))).Append("</a>");
        if (hint is not null)
        {
            sb.Append("<div class=\"art-hint\">").Append(Enc(hint)).Append("</div>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    private static string Legend(string cls, string label, int count) =>
        $"<div class=\"leg leg-{cls}\"><span class=\"leg-dot\"></span><span class=\"leg-label\">{Enc(label)}</span><span class=\"leg-count\">{count.ToString(Inv)}</span></div>";

    private static string Kpi(string label, string value, string cls, string? filter = null)
    {
        if (filter is null)
        {
            return $"<div class=\"kpi kpi-{cls}\"><div class=\"kpi-value\">{Enc(value)}</div><div class=\"kpi-label\">{Enc(label)}</div></div>";
        }

        return $"<div class=\"kpi kpi-{cls} clickable\" data-filter=\"{filter}\" role=\"button\" tabindex=\"0\" title=\"Show {Enc(label)} tests\">" +
               $"<div class=\"kpi-value\">{Enc(value)}</div><div class=\"kpi-label\">{Enc(label)}</div><div class=\"kpi-hint\">filter ▾</div></div>";
    }

    private static string ShortClass(string fullName)
    {
        var idx = fullName.LastIndexOf('.');
        return idx >= 0 && idx < fullName.Length - 1 ? fullName[(idx + 1)..] : fullName;
    }

    /// <summary>
    /// Derives a per-step pass/fail label for the table. Actions log their name before running and
    /// only continue on success, so for a failed test the last body step is where it stopped;
    /// everything before it succeeded. Steps logged during teardown (index &gt;= <paramref name="bodyCount"/>)
    /// are treated as context so trace/video lines are never mistaken for the failure.
    /// </summary>
    private static (string Label, string Cls)[] ComputeStepStatuses(IReadOnlyList<StepRecord> steps, TestOutcome outcome, int bodyCount)
    {
        var result = new (string, string)[steps.Count];
        var boundary = bodyCount >= 0 ? bodyCount : steps.Count;

        static bool IsBanner(string m) =>
            m.StartsWith("=====", StringComparison.Ordinal)
            || m.StartsWith("Test PASSED", StringComparison.Ordinal)
            || m.StartsWith("Test FAILED", StringComparison.Ordinal)
            || m.StartsWith("Test SKIPPED", StringComparison.Ordinal);

        var failPoint = -1;
        if (outcome == TestOutcome.Failed)
        {
            for (var i = 0; i < boundary; i++)
            {
                if (steps[i].Status == StepStatus.Fail && !IsBanner(steps[i].Message))
                {
                    failPoint = i;
                    break;
                }
            }
            if (failPoint == -1)
            {
                for (var i = boundary - 1; i >= 0; i--)
                {
                    if (!IsBanner(steps[i].Message))
                    {
                        failPoint = i;
                        break;
                    }
                }
            }
        }

        for (var i = 0; i < steps.Count; i++)
        {
            var s = steps[i];
            var banner = IsBanner(s.Message);

            if (i == failPoint)
            {
                result[i] = ("FAILED", "fail");
            }
            else if (i >= boundary)
            {
                result[i] = ("INFO", "info");
            }
            else if (s.Status == StepStatus.Fail && !banner)
            {
                result[i] = ("FAILED", "fail");
            }
            else if (s.Status == StepStatus.Warning)
            {
                result[i] = ("WARN", "warn");
            }
            else if (banner)
            {
                result[i] = ("INFO", "info");
            }
            else if (outcome == TestOutcome.Failed)
            {
                result[i] = i < failPoint ? ("PASSED", "pass") : ("INFO", "info");
            }
            else if (outcome == TestOutcome.Skipped)
            {
                result[i] = ("INFO", "info");
            }
            else
            {
                result[i] = ("PASSED", "pass");
            }
        }

        return result;
    }

    private static string FormatDuration(TimeSpan d)
    {
        if (d <= TimeSpan.Zero)
        {
            return "0 ms";
        }
        if (d.TotalMilliseconds < 1000)
        {
            return $"{(int)d.TotalMilliseconds} ms";
        }
        if (d.TotalSeconds < 60)
        {
            return string.Create(Inv, $"{d.TotalSeconds:0.0} s");
        }
        if (d.TotalMinutes < 60)
        {
            return $"{d.Minutes}m {d.Seconds}s";
        }
        return $"{(int)d.TotalHours}h {d.Minutes}m";
    }

    private static string Enc(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

    private const string Css = """
        :root{
          --bg:#0f1420; --bg-2:#151b2b; --panel:#1a2133; --panel-2:#202a41; --border:#2a3550;
          --text:#e7ecf5; --muted:#93a1bd; --pass:#22c55e; --fail:#ef4444; --skip:#f59e0b;
          --info:#60a5fa; --accent:#7c9cff; --shadow:0 10px 30px rgba(0,0,0,.35);
        }
        [data-theme="light"]{
          --bg:#f4f6fb; --bg-2:#ffffff; --panel:#ffffff; --panel-2:#f7f9fe; --border:#e2e8f4;
          --text:#1a2338; --muted:#5b6b8c; --shadow:0 10px 30px rgba(30,50,90,.10);
        }
        *{box-sizing:border-box}
        body{margin:0;font-family:'Segoe UI',system-ui,-apple-system,Roboto,Helvetica,Arial,sans-serif;
          background:radial-gradient(1200px 600px at 80% -10%,var(--bg-2),var(--bg)) fixed;color:var(--text);}
        .container{max-width:1160px;margin:0 auto;padding:24px 20px 60px;}
        .topbar{position:sticky;top:0;z-index:20;display:flex;align-items:center;justify-content:space-between;
          padding:14px 20px;background:color-mix(in srgb,var(--bg) 82%,transparent);backdrop-filter:blur(10px);
          border-bottom:1px solid var(--border);}
        .brand{display:flex;align-items:center;gap:12px}
        .logo{display:grid;place-items:center;width:38px;height:38px;border-radius:11px;font-weight:800;color:#fff;
          background:linear-gradient(135deg,#7c9cff,#4f7cff);box-shadow:0 6px 16px rgba(79,124,255,.45)}
        .brand-title{font-weight:700;font-size:16px;letter-spacing:.2px}
        .brand-sub{font-size:12px;color:var(--muted)}
        .icon-btn{border:1px solid var(--border);background:var(--panel);color:var(--text);width:40px;height:40px;
          border-radius:10px;font-size:18px;cursor:pointer;transition:.15s}
        .icon-btn:hover{border-color:var(--accent);transform:translateY(-1px)}

        .dashboard{display:grid;grid-template-columns:320px 1fr;gap:18px;margin-top:22px}
        @media(max-width:820px){.dashboard{grid-template-columns:1fr}}
        .donut-card,.panel,.kpi,.toolbar,.test{background:var(--panel);border:1px solid var(--border);border-radius:16px}
        .donut-card{padding:22px;box-shadow:var(--shadow);display:flex;flex-direction:column;align-items:center;gap:16px}
        .donut{width:190px;height:190px;border-radius:50%;display:grid;place-items:center;position:relative;
          transition:transform .3s}
        .donut:hover{transform:rotate(2deg) scale(1.02)}
        .donut-hole{width:132px;height:132px;border-radius:50%;background:var(--panel);display:grid;place-items:center;
          box-shadow:inset 0 0 0 1px var(--border)}
        .donut-rate{font-size:34px;font-weight:800;line-height:1}
        .donut-label{font-size:12px;color:var(--muted);text-transform:uppercase;letter-spacing:1px}
        .legend{display:flex;gap:16px;flex-wrap:wrap;justify-content:center}
        .leg{display:flex;align-items:center;gap:7px;font-size:13px}
        .leg-dot{width:11px;height:11px;border-radius:3px}
        .leg-pass .leg-dot{background:var(--pass)} .leg-fail .leg-dot{background:var(--fail)} .leg-skip .leg-dot{background:var(--skip)}
        .leg-count{font-weight:700}
        .leg-label{color:var(--muted)}

        .kpis{display:grid;grid-template-columns:repeat(3,1fr);gap:14px}
        @media(max-width:520px){.kpis{grid-template-columns:repeat(2,1fr)}}
        .kpi{padding:18px 18px;box-shadow:var(--shadow);position:relative;overflow:hidden}
        .kpi:before{content:"";position:absolute;left:0;top:0;bottom:0;width:4px;background:var(--accent)}
        .kpi-pass:before{background:var(--pass)} .kpi-fail:before{background:var(--fail)}
        .kpi-skip:before{background:var(--skip)} .kpi-rate:before{background:var(--info)}
        .kpi-time:before{background:#a78bfa} .kpi-total:before{background:var(--accent)}
        .kpi-value{font-size:30px;font-weight:800;line-height:1}
        .kpi-label{margin-top:6px;color:var(--muted);font-size:13px;text-transform:uppercase;letter-spacing:.5px}
        .kpi.clickable{cursor:pointer;transition:.15s}
        .kpi.clickable:hover{transform:translateY(-2px);border-color:var(--accent)}
        .kpi.clickable:focus-visible{outline:2px solid var(--accent);outline-offset:2px}
        .kpi-hint{margin-top:8px;font-size:10px;letter-spacing:.6px;text-transform:uppercase;color:var(--accent);opacity:0;transition:.15s}
        .kpi.clickable:hover .kpi-hint,.kpi.clickable:focus-visible .kpi-hint{opacity:.9}

        .panel{padding:18px 20px;margin-top:18px;box-shadow:var(--shadow)}
        .panel h2{margin:0 0 14px;font-size:15px;text-transform:uppercase;letter-spacing:1px;color:var(--muted)}
        .env-grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(220px,1fr));gap:10px}
        .env-item{display:flex;flex-direction:column;gap:2px;padding:10px 12px;background:var(--panel-2);border:1px solid var(--border);border-radius:10px}
        .env-key{font-size:11px;text-transform:uppercase;letter-spacing:.6px;color:var(--muted)}
        .env-val{font-weight:600;word-break:break-word}

        .toolbar{display:flex;flex-wrap:wrap;gap:12px;align-items:center;justify-content:space-between;padding:12px 14px;margin-top:18px;box-shadow:var(--shadow)}
        .search{flex:1 1 240px}
        .search input{width:100%;padding:10px 14px;border-radius:10px;border:1px solid var(--border);background:var(--panel-2);color:var(--text);font-size:14px}
        .search input:focus{outline:none;border-color:var(--accent)}
        .filters{display:flex;gap:8px;flex-wrap:wrap}
        .chip{border:1px solid var(--border);background:var(--panel-2);color:var(--text);padding:8px 12px;border-radius:999px;cursor:pointer;font-size:13px;transition:.15s}
        .chip b{opacity:.7;margin-left:4px}
        .chip:hover{border-color:var(--accent)}
        .chip.active{background:var(--accent);border-color:var(--accent);color:#0b1020}
        .chip.active b{opacity:.85}
        .ghost-btn{border:1px solid var(--border);background:transparent;color:var(--muted);padding:8px 10px;border-radius:8px;cursor:pointer;font-size:13px}
        .ghost-btn:hover{color:var(--text);border-color:var(--accent)}

        .tests{margin-top:16px;display:flex;flex-direction:column;gap:12px}
        .empty{padding:40px;text-align:center;color:var(--muted);border:1px dashed var(--border);border-radius:14px}
        .test{overflow:hidden;box-shadow:var(--shadow);border-left:4px solid var(--border)}
        .test.passed{border-left-color:var(--pass)} .test.failed{border-left-color:var(--fail)} .test.skipped{border-left-color:var(--skip)}
        .test-head{width:100%;display:flex;align-items:center;gap:12px;padding:14px 16px;background:transparent;border:none;color:var(--text);cursor:pointer;text-align:left;font-size:14px}
        .test-head:hover{background:var(--panel-2)}
        .pill{font-size:11px;font-weight:800;letter-spacing:.6px;padding:5px 9px;border-radius:7px;color:#0b1020;white-space:nowrap}
        .pill.passed{background:var(--pass)} .pill.failed{background:var(--fail)} .pill.skipped{background:var(--skip)}
        .test-name{flex:1;font-weight:600;display:flex;align-items:baseline;gap:10px;flex-wrap:wrap}
        .test-class{font-size:12px;color:var(--muted);font-weight:500}
        .test-meta{display:flex;align-items:center;gap:10px;color:var(--muted);font-size:12px}
        .tag{background:var(--panel-2);border:1px solid var(--border);padding:2px 8px;border-radius:999px}
        .dur{font-variant-numeric:tabular-nums}
        .chevron{transition:.2s;display:inline-block}
        .test.open .chevron{transform:rotate(180deg)}
        .test-body{display:none;padding:0 16px 18px;border-top:1px solid var(--border)}
        .test.open .test-body{display:block}
        .desc{color:var(--muted);margin:14px 0 6px}
        .timing{color:var(--muted);font-size:12px;margin:10px 0 4px;font-variant-numeric:tabular-nums}

        .error{margin:14px 0;border:1px solid color-mix(in srgb,var(--fail) 40%,var(--border));border-radius:12px;overflow:hidden}
        .error-msg{padding:12px 14px;background:color-mix(in srgb,var(--fail) 14%,transparent);color:var(--text);font-weight:600}
        .stack{margin:0;padding:12px 14px;font-size:12px;line-height:1.55;overflow:auto;max-height:280px;color:var(--muted);
          font-family:'Cascadia Code',Consolas,monospace;white-space:pre-wrap;word-break:break-word}

        .steps-head{margin:16px 0 8px;font-weight:700;font-size:13px;text-transform:uppercase;letter-spacing:.6px;color:var(--muted)}
        .steps-head .count{background:var(--panel-2);border:1px solid var(--border);border-radius:999px;padding:1px 8px;margin-left:6px}
        .step-table-wrap{border:1px solid var(--border);border-radius:12px;overflow:hidden}
        .step-table{width:100%;border-collapse:collapse;font-size:13px}
        .step-table th,.step-table td{padding:9px 12px;text-align:left;border-bottom:1px solid var(--border);vertical-align:top}
        .step-table thead th{background:var(--panel-2);color:var(--muted);text-transform:uppercase;font-size:11px;letter-spacing:.6px;position:sticky;top:0}
        .step-table tbody tr:last-child td{border-bottom:none}
        .step-table tbody tr:hover{background:var(--panel-2)}
        .step-table tr.row-fail{background:color-mix(in srgb,var(--fail) 12%,transparent)}
        .step-num{color:var(--muted);font-variant-numeric:tabular-nums;width:40px;text-align:right}
        .step-msg-cell{word-break:break-word}
        .step-col-status{width:1%}
        .step-col-time,.step-col-dur{white-space:nowrap;font-variant-numeric:tabular-nums;color:var(--muted);width:1%}
        .st{display:inline-block;font-size:11px;font-weight:800;letter-spacing:.4px;padding:3px 9px;border-radius:6px;white-space:nowrap}
        .st-pass{background:color-mix(in srgb,var(--pass) 20%,transparent);color:var(--pass)}
        .st-fail{background:color-mix(in srgb,var(--fail) 24%,transparent);color:var(--fail)}
        .st-warn{background:color-mix(in srgb,var(--skip) 22%,transparent);color:var(--skip)}
        .st-info{background:var(--panel-2);color:var(--muted)}

        .artifacts{display:flex;flex-wrap:wrap;gap:16px;margin-top:16px}
        .art-block{display:flex;flex-direction:column;gap:6px}
        .art-label{font-size:11px;text-transform:uppercase;letter-spacing:.6px;color:var(--muted)}
        .shot{max-width:340px;max-height:220px;border-radius:10px;border:1px solid var(--border);cursor:zoom-in;transition:.2s}
        .shot:hover{transform:scale(1.01);border-color:var(--accent)}
        .art-link{color:var(--accent);text-decoration:none;font-weight:600;border:1px solid var(--border);padding:8px 12px;border-radius:9px;background:var(--panel-2)}
        .art-link:hover{border-color:var(--accent)}
        .art-hint{font-size:11px;color:var(--muted)}

        .footer{text-align:center;color:var(--muted);font-size:12px;padding:26px 20px 40px}

        .lightbox{display:none;position:fixed;inset:0;z-index:50;background:rgba(3,6,14,.86);align-items:center;justify-content:center;padding:30px;cursor:zoom-out}
        .lightbox.open{display:flex}
        .lightbox img{max-width:96vw;max-height:92vh;border-radius:12px;box-shadow:0 20px 60px rgba(0,0,0,.6)}
        """;

    private const string Script = """
        (function(){
          var root=document.documentElement;
          var saved=localStorage.getItem('testhon-theme');
          if(saved){root.setAttribute('data-theme',saved);}
          document.getElementById('themeToggle').addEventListener('click',function(){
            var next=root.getAttribute('data-theme')==='dark'?'light':'dark';
            root.setAttribute('data-theme',next);localStorage.setItem('testhon-theme',next);
          });

          document.querySelectorAll('.test-head').forEach(function(h){
            h.addEventListener('click',function(){h.parentElement.classList.toggle('open');});
          });
          var exp=document.getElementById('expandAll'),col=document.getElementById('collapseAll');
          if(exp)exp.addEventListener('click',function(){document.querySelectorAll('.test').forEach(function(t){if(t.style.display!=='none')t.classList.add('open');});});
          if(col)col.addEventListener('click',function(){document.querySelectorAll('.test').forEach(function(t){t.classList.remove('open');});});

          var currentFilter='all',term='';
          function apply(){
            document.querySelectorAll('.test').forEach(function(t){
              var okStatus=currentFilter==='all'||t.getAttribute('data-status')===currentFilter;
              var okTerm=term===''||t.getAttribute('data-name').indexOf(term)>-1;
              t.style.display=(okStatus&&okTerm)?'':'none';
            });
          }
          function setFilter(f){
            currentFilter=f;
            document.querySelectorAll('.chip').forEach(function(x){
              x.classList.toggle('active',x.getAttribute('data-filter')===f);
            });
            apply();
          }
          document.querySelectorAll('.chip').forEach(function(c){
            c.addEventListener('click',function(){setFilter(c.getAttribute('data-filter'));});
          });
          function gotoTests(){
            var el=document.getElementById('tests');
            if(el)el.scrollIntoView({behavior:'smooth',block:'start'});
          }
          document.querySelectorAll('.kpi.clickable').forEach(function(k){
            function act(){setFilter(k.getAttribute('data-filter'));gotoTests();}
            k.addEventListener('click',act);
            k.addEventListener('keydown',function(e){
              if(e.key==='Enter'||e.key===' '){e.preventDefault();act();}
            });
          });
          var s=document.getElementById('search');
          if(s)s.addEventListener('input',function(){term=s.value.toLowerCase().trim();apply();});

          var lb=document.getElementById('lightbox'),lbImg=document.getElementById('lightbox-img');
          document.querySelectorAll('.shot').forEach(function(img){
            img.addEventListener('click',function(){lbImg.src=img.src;lb.classList.add('open');});
          });
          if(lb)lb.addEventListener('click',function(){lb.classList.remove('open');lbImg.src='';});
          document.addEventListener('keydown',function(e){if(e.key==='Escape'&&lb)lb.classList.remove('open');});
        })();
        """;
}
