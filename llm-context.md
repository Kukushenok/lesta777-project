I'm making a Unity 6000.2.12f1 game. There is a set of rules in which our codebase runs:
CORE FORMATTING
- UTF-8, LF line endings, trim whitespace, final newline
- 4-space indentation for C#
BRACES & NEWLINES
- New lines before {, else, catch, finally
- Braces required (warning)
SPACING
- Space after control flow keywords
- No spaces in empty parameter lists ()
- No space after casts
CODE STYLE
- var for built-in/types apparent (suggestion)
- Modern features: switch/throw expressions, pattern matching (suggestion)
- using outside namespace (warning)
- No this. qualification
ACCESSIBILITY
- Explicit access modifiers required (warning)
- readonly fields preferred (warning)
- Modifier order enforced (warning)
NAMING CONVENTIONS
- Types: PascalCase (warning)
- Non-private members: PascalCase (error for public fields)
- Private fields: _camelCase (warning)
- Constants: PascalCase (warning)
- Locals/parameters: camelCase (suggestion)
ANALYZER CONFIG
- Style/Naming rules: warning severity
- Unity analyzer rules: warnings enabled
- Documentation rules disabled
ENFORCEMENT
- Mixed severity: warnings (must fix) + suggestions (should fix)
- Public fields prohibited via naming rules
- Modern C# patterns encouraged
OTHER
- Use protected modifier for each Unity message (Awake, Update, etc.)
- The default namespace is Game namespace
- Use UniTask over Coroutines when possible.
- Use New Input System over old Input System. In order to use specific InputActions fetch them from InputActionAsset

If you have any questions, ask me before doing the task.

{пишите свой запрос здесь}