# AI prompt: Generate reviewable login scenarios

## Purpose

Use this prompt to turn approved acceptance criteria into candidate login test scenarios. The output is a design aid: a quality engineer reviews and approves it before any scenario is added to the automated suite.

## Reusable prompt

```text
Act as a senior quality engineer. Convert the acceptance criteria below into a
concise, risk-based login test scenario set.

Constraints:
- Do not invent application behavior, selectors, URLs, credentials, or
  requirements. Mark missing information as an open question.
- Do not include real customer data, secrets, or production credentials.
- Include positive, negative, boundary, accessibility, and recovery cases only
  when supported by the supplied requirements.
- Separate observed requirements from assumptions.
- Do not write executable code unless explicitly requested.

For each scenario, provide:
1. A stable scenario ID and concise title.
2. Risk/priority and the user or business impact.
3. Preconditions and safe test-data needs.
4. Action steps and observable expected results.
5. Automation suitability and any reason to keep it manual.
6. Requirement traceability, or "unmapped" if none is supplied.

Return a Markdown table followed by open questions and assumptions.

Acceptance criteria:
<PASTE APPROVED, NON-SENSITIVE REQUIREMENTS HERE>
```

## Reviewed example for this repository

The following illustrative scenarios are based on the public SauceDemo sample login behavior. They are maintained as a human-reviewed example; the pipeline does not call an AI service or generate tests at runtime.

| ID | Scenario | Expected result | Automation fit |
|---|---|---|---|
| LOGIN-001 | Sign in as `standard_user` with the public demo password | User reaches the Products inventory page | High; stable critical user journey |
| LOGIN-002 | Sign in as `locked_out_user` with the public demo password | A clear locked-out error is displayed and inventory is not opened | High; negative access-control behavior |
| LOGIN-003 | Submit the form with an empty username | Required-field validation is displayed | Candidate; verify current application behavior before implementation |

## Human review checklist

- Confirm each expected result against approved requirements or the target application's behavior.
- Remove unsupported assumptions and clarify open questions with the product owner.
- Use only approved, non-sensitive test data.
- Review risk, accessibility, and test-data impact before automation.
- Treat AI output as a proposal; a qualified engineer owns the final scenario and code.
