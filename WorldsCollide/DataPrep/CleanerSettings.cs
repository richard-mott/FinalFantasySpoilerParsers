namespace FinalFantasySpoilerParsers.WorldsCollide.DataPrep;

internal static class CleanerSettings
{
    internal const int ColumnSize = 60;
    internal const int ValidationIndex = 55;
    internal const int ValidationLength = 5;

    internal static string LeftColumnPadding => new string(' ', 60);
    internal static string ValidationString => new string(' ', ValidationLength);
    internal static string SectionStart => new string('-', 5);
    
    internal const string SectionRegex =
        @"-{5,} (?<SectionName>[\w\s]+) -{5,}.*?" +
        @"(?<SectionText>^.*?)^-";

    internal const string GameHeading = @"Game";
    internal const string PartyHeading = @"Party";
    internal const string BattleHeading = @"Battle";
    internal const string MagicHeading = @"Magic";
    internal const string ItemsHeading = @"Items";
    internal const string CustomHeading = @"Custom";
    internal const string OtherHeading = @"Other";
    internal const string EventsHeading = @"Events";
    internal const string CommandsHeading = @"Commands";
    internal const string NaturalMagicHeading = @"Natural Magic";
    internal const string ChestsHeading = @"Chests";
    internal const string LoresHeading = @"Lores";
    internal const string StartRagesHeading = @"Start Rages";
    internal const string DancesHeading = @"Dances";
    internal const string EspersHeading = @"Espers";
    internal const string ShopsHeading = @"Shops";
    internal const string ColiseumHeading = @"Coliseum";
}
