using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Data.Json;
using Windows.Storage;

namespace Venz.UI.Xaml.Common
{
    public sealed partial class ChangesPage: Page
    {
        public ChangesPage()
        {
            InitializeComponent();
        }

        protected override async void SetState(FrameNavigation.Parameter navigationParameter, FrameNavigation.Parameter stateParameter)
        {
            base.SetState(navigationParameter, stateParameter);
            var changelogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Changelog.json"));
            var changelogString = await FileIO.ReadTextAsync(changelogFile);
            var releases = new List<Release>();
            foreach (var releaseItem in JsonArray.Parse(changelogString))
            {
                var release = releaseItem.GetObject();
                var version = release["version"].GetString();
                var date = DateTime.Parse(release["date"].GetString());
                var sections = new List<ReleaseSection>();
                foreach (var sectionItem in release["sections"].GetArray())
                {
                    var section = sectionItem.GetObject();
                    var title = section["title"].GetString();
                    var changes = new List<String>();
                    foreach (var changeItem in section["changes"].GetArray())
                        changes.Add(changeItem.GetString());
                    sections.Add(new ReleaseSection(title, changes));
                }
                releases.Add(new Release(version, date, sections));
            }
            ReleaseListControl.ItemsSource = releases;
        }
    }

    public class Release
    {
        public String Version { get; }
        public String Date { get; }
        public IEnumerable<ReleaseSection> Sections { get; }

        public Release(String version, DateTime date, IEnumerable<ReleaseSection> sections)
        {
            Version = $"Version {version}";
            Date = date.ToString($"{DateTimeFormatInfo.CurrentInfo.MonthDayPattern}, yyyy");
            Sections = sections;
        }
    }

    public class ReleaseSection
    {
        public String Title { get; }
        public IEnumerable<String> Changes { get; }

        public ReleaseSection(String title, IEnumerable<String> changes)
        {
            Title = title.ToUpper();
            Changes = changes;
        }
    }
}
