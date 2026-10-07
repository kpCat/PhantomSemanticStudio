# Правила работы с Phantom Semantic Studio

Рабочий root: C:\Users\ZBook\PhantomSemanticStudio\.
Это НЕ часть kpCat/L2J. Не распаковывать и не создавать git-ветку программы внутри L2J.

## Неприкосновенная область
C:\Users\ZBook\L2J_Mobius\ целиком, включая High Five, его dist, .phantom-local, серверы, базы, geodata, конфиги и текущий git worktree. Только точечное чтение humanized-каталога/загрузчика. Ни runtime, ни Codex не записывают туда.

## Безопасность программы
- Нет метода/кнопки установки, автопубликации, overwrite core/custom, auto-approve, auto-repair.
- Все записи только в собственный workspace; source/workspace disjoint; reparse guards.
- Экспорт всегда REVIEW_ONLY / Java validator NOT_RUN до отдельного доказательства. Не выдавать предложенный XML за прошедший штатную проверку.
- Модель не генерирует/исполняет код, не получает tools и filesystem. Один bounded JSON-запрос без автоматических повторов.
- Правка отменяет approval; approval связан с content hash и source fingerprint. Нет автоматического rebase кандидатов.
- Не скрывать ошибки/пропущенные проверки под GREEN.

## WinForms
MainForm.cs / MainForm.Designer.cs / MainForm.resx. Standard controls. Constructor только InitializeComponent. Статическая явная инициализация, layout и events в InitializeComponent; никаких циклов, условного UI, LINQ, lambdas, I/O, async, DI и вспомогательных фабрик controls. Не переносить форму в runtime BuildUi.

## Проверки и Git
Сначала docs/DESIGN_RU.md, docs/SOURCE_AUDIT_RU.md, reports/BASELINE_VERIFICATION.md и выбранная задача.
Минимум scripts/Build-Verify.ps1 + реальные targeted проверки задачи. Не запускать L2J Ant/JAR/сервер/БД и не гонять unrelated suites.
Git только в собственном root программы. Сохранять user dirt. Без reset/clean/stash/rebase/amend/force и git add . .
Не использовать origin kpCat/L2J для нового приложения. Если собственного origin нет — PUSH_NOT_CONFIGURED, review ZIP обязателен.
