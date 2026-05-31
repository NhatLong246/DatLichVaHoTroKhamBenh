namespace HeThongDatLichVaKhamBenh.Services;

public interface IGeminiService
{
    Task<GeminiSuggestionResponse?> SuggestSpecialtiesAsync(string symptoms, IEnumerable<SpecialtyInfo> availableSpecialties);
}

public record SpecialtyInfo(string Id, string Name, int AverageMinutes);

public record GeminiSuggestionResponse(
    List<GeminiSpecialtySuggestion> Suggestions,
    bool HasUrgentWarning,
    string Message
);

public record GeminiSpecialtySuggestion(
    string SpecialtyId,
    string SpecialtyName,
    int EstimatedMinutes,
    string Reason
);
