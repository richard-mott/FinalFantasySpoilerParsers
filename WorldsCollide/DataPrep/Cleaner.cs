using System.Text;
using System.Text.RegularExpressions;

namespace FinalFantasySpoilerParsers.WorldsCollide.DataPrep;

internal class Cleaner
{
    internal string SeedDetails { get; private set; }
    internal string Game { get; private set; }
    internal string Party { get; private set; }
    internal string Battle { get; private set; }
    internal string Magic { get; private set; }
    internal string Items { get; private set; }
    internal string Custom { get; private set; }
    internal string Other { get; private set; }
    internal string Events { get; private set; }
    internal string Commands { get; private set; }
    internal string NaturalMagic { get; private set; }
    internal string Chests { get; private set; }
    internal string Lores { get; private set; }
    internal string StartRages { get; private set; }
    internal string Dances { get; private set; }
    internal string Espers { get; private set; }
    internal string Shops { get; private set; }
    internal string Coliseum { get; private set; }

    internal Cleaner(string spoilerText)
    {
        var singleColumnText = CreateSingleColumnText(spoilerText);

        DistributeText(singleColumnText);
    }

    #region Single Column Conversion

    // Create a single-column string from the spoiler text
    private string CreateSingleColumnText(string spoilerText)
    {
        var resultBuilder = new StringBuilder();
        var leftColumnBuilder = new StringBuilder();
        var rightColumnBuilder = new StringBuilder();

        // Process each line of the spoiler
        var splitText = spoilerText.Split(
            Environment.NewLine.ToCharArray(),
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in splitText)
        {
            ProcessLine(line, resultBuilder, leftColumnBuilder, rightColumnBuilder);
        }

        // Append any residual values NewLine characters, if necessary
        if (leftColumnBuilder.Length > 0)
        {
            resultBuilder.Append(leftColumnBuilder);
            if (!resultBuilder.ToString().EndsWith(Environment.NewLine))
            {
                resultBuilder.AppendLine();
            }
        }

        if (rightColumnBuilder.Length > 0)
        {
            resultBuilder.Append(rightColumnBuilder);
            if (!resultBuilder.ToString().EndsWith(Environment.NewLine))
            {
                resultBuilder.AppendLine();
            }
        }

        // Append a final '-' to assist in regex parsing
        resultBuilder.Append('-');

        return resultBuilder.ToString();
    }

    // Process a single line of text from the spoiler
    private void ProcessLine(string line, StringBuilder resultBuilder, StringBuilder leftColumnBuilder,
        StringBuilder rightColumnBuilder)
    {
        if (line.StartsWith(CleanerSettings.SectionStart))
        {
            ProcessLine(line, resultBuilder, leftColumnBuilder, rightColumnBuilder);
        }
        else if (line.Length < CleanerSettings.ColumnSize)
        {
            ProcessShortLine(line, leftColumnBuilder);
        }
        else
        {
            ProcessLongLine(line, leftColumnBuilder, rightColumnBuilder);
        }
    }

    // Add the contents of the previous section to the result builder and start the new section
    private void ProcessNewSection(string line, StringBuilder resultBuilder, StringBuilder leftColumnBuilder,
        StringBuilder rightColumnBuilder)
    {
        // Add the left column contents and clear it
        if (leftColumnBuilder.Length > 0)
        {
            resultBuilder.Append(leftColumnBuilder);
            leftColumnBuilder.Clear();
        }

        // Add the right column contents and clear it
        if (rightColumnBuilder.Length > 0)
        {
            resultBuilder.Append(rightColumnBuilder);
            rightColumnBuilder.Clear();
        }

        // Add the new section header
        resultBuilder
            .Append(line.TrimEnd())
            .AppendLine();
    }

    // If the line is shorter than {CleanerSettings.ColumnSize}, then there is only a left column
    private void ProcessShortLine(string line, StringBuilder leftColumnBuilder)
    {
        leftColumnBuilder
            .Append(line.TrimEnd())
            .AppendLine();
    }

    // Process lines that are greater than {CleanerSettings.ColumnSize} in length
    private void ProcessLongLine(string line, StringBuilder leftColumnBuilder, StringBuilder rightColumnBuilder)
    {
        if (HasTwoColumns(line))
        {
            // The left column is the first {CleanerSettings.ColumnSize} characters of the line
            var leftColumn = line[..CleanerSettings.ColumnSize];

            // The right column is everything after the first {CleanerSettings.ColumnSize} characters
            var rightColumn = line[CleanerSettings.ColumnSize..];

            // Add the left column if it's not empty
            if (leftColumn != CleanerSettings.LeftColumnPadding)
            {
                leftColumnBuilder
                    .Append(leftColumn.TrimEnd())
                    .AppendLine();
            }

            // Add the right column
            rightColumnBuilder
                .Append(rightColumn.TrimEnd())
                .AppendLine();
        }
        else
        {
            // Single column, so append it to the left builder
            leftColumnBuilder
                .Append(line.TrimEnd())
                .AppendLine();
        }
    }

    // Not all lines greater than {CleanerSettings.ColumnSize} in length are two columns
    private bool HasTwoColumns(string line)
    {
        // If there are two columns, the second column always begins at {CleanerSettings.ColumnSize}
        // and is immediately preceded by five spaces
        var validationSubstring = line.Substring(
            CleanerSettings.ValidationIndex, CleanerSettings.ValidationLength);
        
        return (validationSubstring == CleanerSettings.ValidationString);
    }

    #endregion
    
    #region Distribute Text to Properties

    // Distribute each section of the spoiler to the appropriate member variables
    private void DistributeText(string singleColumnText)
    {
        // Spoiler details are at the beginning of the text, until the first section definition
        var firstSectionIndex = singleColumnText
            .IndexOf(CleanerSettings.SectionStart, StringComparison.Ordinal);

        // Validate that the spoiler contains section headings
        if (firstSectionIndex == -1)
        {
            throw new ArgumentException(@"Imported text does not contain section headings.");
        }

        SeedDetails = singleColumnText[..firstSectionIndex];

        // Everything after the seed details is split into separate sections
        var sectionsText = singleColumnText[firstSectionIndex..];
        ProcessSections(sectionsText);
    }

    private void ProcessSections(string sectionsText)
    {
        var regex = new Regex(CleanerSettings.SectionRegex,
            RegexOptions.Multiline | RegexOptions.Singleline);
        var matches = regex.Matches(sectionsText);

        foreach (Match match in matches)
        {
            var groups = match.Groups;
            var sectionName = groups["SectionName"].Value;
            var sectionText = groups["SectionText"].Value;

            SetSectionTextBySectionName(sectionName, sectionText);
        }
    }

    private void SetSectionTextBySectionName(string sectionName, string sectionText)
    {
        switch (sectionName)
        {
            case CleanerSettings.GameHeading:
                Game = sectionText;
                break;
            case CleanerSettings.PartyHeading:
                Party = sectionText;
                break;
            case CleanerSettings.BattleHeading:
                Battle = sectionText;
                break;
            case CleanerSettings.MagicHeading:
                Magic = sectionText;
                break;
            case CleanerSettings.ItemsHeading:
                Items = sectionText;
                break;
            case CleanerSettings.CustomHeading:
                Custom = sectionText;
                break;
            case CleanerSettings.OtherHeading:
                Other = sectionText;
                break;
            case CleanerSettings.EventsHeading:
                Events = sectionText;
                break;
            case CleanerSettings.CommandsHeading:
                Commands = sectionText;
                break;
            case CleanerSettings.NaturalMagicHeading:
                NaturalMagic = sectionText;
                break;
            case CleanerSettings.ChestsHeading:
                Chests = sectionText;
                break;
            case CleanerSettings.LoresHeading:
                Lores = sectionText;
                break;
            case CleanerSettings.StartRagesHeading:
                StartRages = sectionText;
                break;
            case CleanerSettings.DancesHeading:
                Dances = sectionText;
                break;
            case CleanerSettings.EspersHeading:
                Espers = sectionText;
                break;
            case CleanerSettings.ShopsHeading:
                Shops = sectionText;
                break;
            case CleanerSettings.ColiseumHeading:
                Coliseum = sectionText;
                break;
            default:
                throw new ArgumentException(
                    $"Unrecognized section name [{sectionName}] in spoiler data.");
        }
    }

    #endregion
}
