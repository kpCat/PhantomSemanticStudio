# Phantom Semantic Studio · 0.1

Отдельная C# WinForms-программа для расширения **разговорного** Semantic Pack High Five под контролем редактора. Текущий checkpoint PSS-004: [reports/PSS-004-final.md](reports/PSS-004-final.md), [UI evidence](reports/PSS-004-ui.md); Java content evidence прежнего PSS-003: [reports/PSS-003-final.md](reports/PSS-003-final.md).

В исходной поставке сборка C#, Visual Studio Designer, LM Studio и Java runtime не запускались. Этот исторический статус сохранён в BASELINE_VERIFICATION.md. PSS-001 выполняет реальные Windows-проверки; LM, Designer, runtime UI/DPI и Java имеют отдельные статусы в итоговом отчёте. Статические проверки структуры не заменяют компиляцию.

## Где распаковать
Создайте `C:\Users\ZBook\PhantomSemanticStudio\` и распакуйте содержимое ZIP туда. В этой папке сразу должны лежать `PhantomSemanticStudio.sln`, `src`, `tests`, `scripts`.
Не распаковывайте программу в `L2J_Mobius` и не заменяйте ею игровой пак.

## Открыть и собрать
Откройте `PhantomSemanticStudio.sln` в Visual Studio 2026 с .NET 10 / .NET desktop development. Startup project: `PhantomSemanticStudio.WinForms`.
Откройте `src\PhantomSemanticStudio.WinForms\MainForm.cs` → «Открыть конструктор».
Контролы находятся в `MainForm.Designer.cs`; конструктор формы не обращается к файлам/API. Все вкладки и controls объявлены статически, без циклов в InitializeComponent.
Модальная форма выбора партии: `StageSelectionForm.cs` → «Открыть конструктор»; `.Designer.cs` и `.resx` вложены в проекте. Её конструктор тоже содержит только `InitializeComponent()`.

Из PowerShell:
```
powershell -ExecutionPolicy Bypass -File .\scripts\Build-Verify.ps1
```
После успешной сборки запускайте F5 или `Start.cmd`. Скрипты ничего не устанавливают в L2J.

## Первый сценарий
1. «Настройки»: уже вписан `C:\Users\ZBook\L2J_Mobius\L2J_Mobius_CT_2.6_HighFive` и `http://127.0.0.1:1234/v1`.
2. В LM Studio запустите локальный API server и загрузите свою модель. Identifier по умолчанию: `gemma-4-26b-a4b-it-ultra-uncensored-heretic`. При включённой авторизации введите token; он остаётся только в памяти процесса.
3. «Проверить LM Studio / модель» делает один GET /v1/models. CHECKED_MODEL_LIST означает только наличие точного ID: при JIT список может содержать скачанную, ещё не загруженную модель. Успешная генерация и качество проверяются отдельно. При SERVER_OFFLINE: LM Studio → Developer → API Server → Start, затем проверьте порт и точный API identifier. AUTH — проверьте token вручную; ENDPOINT — локальный путь /v1; MODEL_NOT_LISTED — исправьте ID вручную. TIMEOUT и CANCELLED различаются; BAD_RESPONSE отклоняет всю партию. Повторов и автоматической подмены модели нет.
4. «Импортировать пак»: читаются имеющиеся humanized v1/v2/v3/custom. Функциональный Semantic Pack не редактируется.
5. «Конструктор»: выберите существующий act/topic, отношения, пол и стиль; напишите пожелания. Тип по умолчанию «Ответ (TEMPLATE)»; «Входная фраза (PATTERN)» создаёт только входы игрока, «Смешанный (MIXED)» допускает оба вида. Выбранный тип ограничивает JSON Schema и повторно проверяется локально у каждого item; неверный kind отклоняет всю партию. Старые GenerationRequest/замечания читаются с MIXED в Core; применение замечания сохраняет явный выбор UI. Партия по умолчанию 6, максимум 20. Контекст включает ограниченную выборку релевантных записей, не весь репозиторий.
6. «Кандидаты»: проверьте, отредактируйте, сохраните и одобрите каждую нужную запись с отметкой ручного ревью. Входные фразы и ответы различаются. Ошибки не принимаются автоматически; неудачная генерация не запускает бесконечных повторов.
7. «Экспорт для ревью»: прежний отдельный ZIP с одобренным JSON, baseline и замечаниями, **REVIEW_ONLY_NOT_SERVER_VALIDATED**. Эта кнопка не создаёт XML. **Java-validator NOT_RUN; не распаковывать поверх сервера.**
8. «Создать изолированное XML-предложение»: в модальном окне вручную отметьте конкретные 1–20 APPROVED из любого числа одобрений. По умолчанию выбор пуст. Поиск по ID/тексту/виду/теме/act показывает первые 500 совпадений; скрытые отметки сохраняются, справа всегда доступен полный выбранный список с исходными текстами и отметками редактора. Старое одобрение или baseline и выбор >20 блокируют продолжение до ручного исправления выбора. Затем отдельно подтвердите точный прокручиваемый список (default «Нет») и редакционную аттестацию (default «Нет»). Отмена ничего не создаёт. Весь выбранный batch повторно проверяется и отклоняется при несовместимой записи; никто не пропускается молча. Только Gender ANY, существующая разговорная topic/act-связка, без placeholders/markup/control, 160 UTF16 после нормализации для PATTERN и 240 UTF-8 bytes для TEMPLATE. Пользователь отдельно проверяет мат/adult, гендерные обороты и ложные игровые утверждения: автоматическое доказательство смысла отсутствует.
9. В `workspace/proposals/<id>/module/dist/game/data/phantoms` создаётся побайтная копия ровно импортированных файлов. Append только в `semantic/custom/my-social-topics.xml` и `conversation/custom/my-phrases.xml`, deterministic IDs и `override="false"`; existing overrides/comments сохраняются. Остальные SHA/bytes неизменны. Source fingerprint проверяется до/после, правка approval блокирует stage. Atomic .partial→finished; receipt хранит ID/hashes без raw текста. Статус **STAGED_UNVALIDATED / Java NOT_RUN / НЕ ДЛЯ УСТАНОВКИ**.

Отдельная явная операторская проверка готового stage:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-PSS003-Java.ps1 -StageRoot <абсолютный путь готового workspace\proposals\id>
```
Скрипт требует JDK25/Ant и локальные libs; ничего не загружает. Физический scratch внутри stage получает только build.xml/java/test/java/test/resources и dependency JAR (без GameServer/LoginServer/sources JAR). Запускаются штатный `phantom-humanized-v3-content-validate` и узкий `loadV3(..., true)` bridge с baseline/stage counters/hash/ID assertions и отдельной corrupt-копией. Все build/temp/reports только внутри scratch. Копирование ограничено 4000 файлами/256 MiB, свободный scratch budget минимум 512 MiB. Доказательство сохраняется в `oracle-<id>/java-validation.json`, связано с exact staged hashes; исходный receipt остаётся STAGED_UNVALIDATED, GUI не присваивает Java PASS автоматически.

**PASS_JAVA_STAGED / JAVA_CONTENT_VALIDATED_NOT_SERVER_READY** означает только успешную загрузку именно этой физической копии. При недоступном окружении **BLOCKED_JAVA**, при отказе loader **FAILED_JAVA**. Изменившиеся source/stage требуют нового staging и проверки. Ни один статус не разрешает установку или публикацию на сервер.

## Диалоговое обучение
«Диалог и обучение» выбирает ответ из импортированного каталога приблизительным C#-инспектором, без вызова Gemma. Показаны pattern/template ID. Это не полная эмуляция сервера: functional-first, identity, социальные gates, gameplay, mood, постоянная память и точный Java selector здесь не воспроизведены.
Объяснение ошибки сохраняется с topic/act/полом/отношениями/стилем и source fingerprint. Во вкладке «Диалог и обучение» можно выбрать и просмотреть сохранённое замечание, затем явно применить только его в конструктор. При смене baseline нужно новое ручное ревью. Старые строковые замечания сохранены; для них scope и baseline неизвестны и требуют проверки. Генерация и каждое одобрение остаются ручными.

При смене кандидата, вкладки или закрытии несохранённый текст и отметка ревью защищены диалогом «Сохранить / Отбросить / Отмена». Правка отменяет approval; неудачное сохранение сохраняет прежнюю корректную state и текст в редакторе.

## Защита исходников
`PackReader` только читает. `WorkspaceStore` пишет лишь в `%LOCALAPPDATA%\PhantomSemanticStudio\workspace`, проверяя непересечение с source/repository. Junction/symlink-пути отклоняются. Второй экземпляр блокируется workspace.lock. JSON сохраняется через временный файл и замену. Это программные guards, не отдельная OS sandbox против злонамеренного процесса, меняющего пути во время записи.
JSON экспорт только в workspace/exports; XML staging только в workspace/proposals. Исполняемые Core/GUI не содержат Process.Start, shell, Ant, Java, Git, сервера или доступа к игровой БД. Java operator script отделён от GUI и проверяет только shadow. Нет метода установки/публикации. Отпечаток источника повторно проверяется; изменение текста аннулирует approval, изменённое одобрение не пропускается молча.
Не меняются ни старые v1/v2, ни действующий v3, ни custom. Расширение реального пака — отдельный будущий проверенный этап.

## Что пока не готово
- Полный визуальный smoke 100%/150% DPI и реальный LM Studio: точные результаты и незакрытые gates приведены в отчётах PSS-002 final/ui. Local build/console/static PASS не заменяет живую Gemma или UI acceptance.
- Полноценный смысловой поиск повторов, грамматический анализ и истинность игровых фактов. Реализованы точные повторы и лексические кандидаты на сходство, не магическая гарантия отсутствия синонимов.
- Полноценная Java/runtime-parity: отдельный content gate не подтверждает поведение игры или условия выбора реплики. Реальные результаты PSS-003 перечислены отдельно в final report.
- Runtime gender/persona-фильтры. JSON сохраняет редакционные ограничения; XML staging блокирует Gender != ANY и любой placeholder.
- Новые act/topic, fact/recall, игровые действия, мат/mature-авторинг, автоматический монтаж manifest, публикация/rollback рабочего сервера. Не добавлять их тихо в первую задачу.
- Развитая база знаний/SQLite: starter использует JSON без внешних NuGet. Это заменяемое хранилище, а не игровая DB.

## Структура
`src/PhantomSemanticStudio.Core` — reader, безопасные пути, workspace, проверка кандидатов, клиент LM Studio, inspector, review export.
`src/PhantomSemanticStudio.WinForms` — стандартная форма и events.
`tests/PhantomSemanticStudio.Tests` — console runner с ненулевым exit code при ошибке; нет пакетов xUnit/NUnit.
`docs` — границы архитектуры и source audit.
`reports/BASELINE_VERIFICATION.md` — честный статус исходной поставки.
`BASELINE_MANIFEST.json` — SHA-256 каждого файла исходной поставки.

После Codex ревью ведётся непосредственно в публичном GitHub main по exact commit/diff, inventory и журналам отчёта. Codex не создаёт review/source ZIP и не запускает New-ReviewBundle. Пользовательский JSON review ZIP приложения сохраняется. Репозиторий L2J не используется как remote нового приложения. STOP после PSS-004; PSS-005 не начат.
