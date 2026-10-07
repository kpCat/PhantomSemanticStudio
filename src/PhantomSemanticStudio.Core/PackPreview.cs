namespace PhantomSemanticStudio.Core;

/// <summary>Инспектор humanized-каталога. Не воспроизводит gameplay/identity/social ownership Java.</summary>
public sealed class PackPreview
{
    private readonly Queue<string> recent = new();
    public void Reset() => recent.Clear();
    public PreviewResult Reply(PackSnapshot snapshot, string input, string band, string register)
    {
        if (TextRules.CodePoints(input) > 256) return Missing("Вход длиннее 256 символов.");
        var text = TextRules.Normalize(input);
        var aliases = snapshot.Entries.Where(e => e.Kind == "ALIAS").ToDictionary(e => e.Id, e => e.Text, StringComparer.Ordinal);
        text = string.Join(" ", text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => aliases.GetValueOrDefault(t, t)));
        string captured = "";
        var match = snapshot.Entries.Where(e => e.Kind == "PATTERN").OrderByDescending(e => e.Priority)
            .ThenByDescending(e => TextRules.Normalize(e.Text).Length).ThenBy(e => e.Id, StringComparer.Ordinal)
            .FirstOrDefault(e => Match(TextRules.Normalize(e.Text), text, out captured));
        if (match == null) return Missing("Совпадение не найдено. Исправление можно оформить кандидатом; автоматического расширения нет.");
        if (TextRules.IsFunctionalAct(match.Act)) return Missing("Это функциональная/identity-ветка. Её проверяет сервер, а не этот инспектор.");
        if (match.Fact.Length > 0 || match.Recall.Length > 0) return new(true, "", match.Id, "", match.Topic, match.Act, "Ветка использует память. Точный ответ здесь не симулируется.");
        var response = snapshot.Entries.Where(e => e.Kind == "TEMPLATE" && e.Act == match.Act && !e.Mature && e.Profanity == "NONE")
            .Where(e => e.Band == "UNKNOWN" || e.Band == band).Where(e => e.Register == "NEUTRAL" || register == "CASUAL")
            .OrderBy(e => e.Id, StringComparer.Ordinal).FirstOrDefault(e => !recent.Contains(e.Id));
        if (response == null) return new(true, "", match.Id, "", match.Topic, match.Act, "Нет подходящей неповторённой реплики; сбросьте историю проверки.");
        recent.Enqueue(response.Id); while (recent.Count > 8) recent.Dequeue();
        var output = response.Text.Replace("{name}", "ТестовыйФантом").Replace("{value}", captured).Replace("{memory}", "[память не подключена]").Replace("{interest}", "[интерес не задан]");
        return new(true, output, match.Id, response.Id, match.Topic, match.Act, "ПРИБЛИЖЁННЫЙ ПРЕДПРОСМОТР: без functional-first, social gates, persona, Java selector и постоянной памяти. Пол не фильтруется.");
    }
    private static bool Match(string pattern, string text, out string value)
    {
        value = "";
        if (!pattern.Contains("{value}")) return pattern == text;
        if (pattern.StartsWith("{value}", StringComparison.Ordinal))
        {
            var suffix = pattern[7..]; if (suffix.Length == 0 || !text.EndsWith(suffix, StringComparison.Ordinal)) return false;
            value = text[..^suffix.Length].Trim(); return value.Length > 0;
        }
        if (pattern.EndsWith("{value}", StringComparison.Ordinal))
        {
            var prefix = pattern[..^7]; if (prefix.Length == 0 || !text.StartsWith(prefix, StringComparison.Ordinal)) return false;
            value = text[prefix.Length..].Trim(); return value.Length > 0;
        }
        return false;
    }
    private static PreviewResult Missing(string reason) => new(false, "", "", "", "", "", reason);
}
