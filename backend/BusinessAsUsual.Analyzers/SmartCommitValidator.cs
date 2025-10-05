using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SmartCommitValidator : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BAU001";
    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Missing Smart Commit Tag",
        messageFormat: "Commit must include a Smart Commit tag like #setup, #docs, #feature",
        category: "CommitEnforcement",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        // Placeholder — actual commit hook integration would go here
    }
}