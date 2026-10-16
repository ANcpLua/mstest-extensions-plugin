# MSTest extensions skill

Keep the skill portable and task-selective. Resolve the consuming project's
actual SDK, framework, adapter, MTP, targets and runner before choosing APIs.
Examples are reproducible evidence, not a required consumer version. Preserve
the distinction between runtime data providers and compile-time generation.

When source changes, use the smallest relevant check. The version verifier
discovers and executes isolated copies; no separate build is needed:

```sh
python3 scripts/verify_versions.py 4.4.1 4.5.1
python3 skills/mstest-extensions/scripts/verify_capabilities.py --mstest-sdk 4.5.1 --hosting --native-aot
python3 scripts/test_inspect.py
python3 scripts/test_install.py
python3 scripts/check_links.py
git diff --check
```

For a new framework version, run the same verifier with the exact SDK version.
Do not weaken checks or overwrite consumer pins to obtain a passing result.
Keep current-run reports in artifacts/ and record only concise, reviewed version
evidence in the references. Validate skill metadata after instruction edits.

Check assertion rejection, original runner failures, data-row names, real async
suspension, joining before disposal, and per-test resource isolation. The probe
project deliberately fails; use its verifier to check exit 2 and exact TRX
outcomes. Reuse passing results until their inputs change. Prose-only changes do
not require executing examples.

Preserve unrelated changes. Commit and push directly to main when publication is
requested. Model evaluations require a user request. Do not imply that package,
source or example checks measure automatic skill selection or model quality.
