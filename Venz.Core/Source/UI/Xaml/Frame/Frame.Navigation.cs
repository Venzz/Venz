using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Animation;

namespace Venz.UI.Xaml
{
    public class FrameNavigation
    {
        private UInt32 LastPageId;
        private Frame Frame;
        private IDictionary<UInt32, Entry> Entries = new Dictionary<UInt32, Entry>();

        internal Boolean IsBackStackResetInitiated;



        public FrameNavigation(Frame frame) { Frame = frame; }

        public void Navigate(Type sourcePageType) => Navigate(sourcePageType, null, null);

        public void Navigate(Type sourcePageType, Parameter parameter) => Navigate(sourcePageType, parameter, null);

        public void Navigate(Type sourcePageType, Parameter parameter, NavigationTransitionInfo infoOverride)
        {
            var pageId = ++LastPageId;
            SetEntry(pageId, new Entry(pageId, new Parameter("navigation", parameter ?? new Parameter())));

            try
            {
                Frame.Navigate(sourcePageType, pageId, infoOverride);
            }
            catch (Exception)
            {
                RemoveEntry(pageId);
                throw;
            }
        }

        public void ResetBackStack() => IsBackStackResetInitiated = true;

        internal Entry GetEntry(UInt32 pageId) => Entries[pageId];

        internal void SetEntry(UInt32 pageId, Entry entry) => Entries.Add(pageId, entry);

        internal void RemoveEntry(UInt32 pageId) => Entries.Remove(pageId);

        internal void ResetBasedOn(UInt32 pageId)
        {
            foreach (var key in new List<UInt32>(Entries.Keys))
                if (key <= pageId)
                    Entries.Remove(key);
        }

        internal void Restore(String state)
        {
            var parameter = Parameter.Parse(state);
            foreach (var token in parameter)
            {
                var entry = new Entry(Convert.ToUInt32(token.Key), (Parameter)token.Value);
                Entries.Add(entry.Id, entry);
                if (LastPageId < entry.Id)
                    LastPageId = entry.Id;
            }
        }

        public override String ToString()
        {
            var parameter = new Parameter();
            foreach (var entry in Entries)
                parameter.Add(entry.Key.ToString(), entry.Value.Parameter);
            return parameter.ToString();
        }

        internal class Entry
        {
            public UInt32 Id { get; }
            public Parameter Parameter { get; internal set; }
            public Entry(UInt32 id, Parameter parameter) { Id = id; Parameter = parameter; }
        }

        public class Parameter: IEnumerable<KeyValuePair<String, Object>>
        {
            private Boolean SingleTokenMode;
            private IDictionary<String, Object> Tokens = new Dictionary<String, Object>();



            public Parameter() { }

            public Parameter(String value)
            {
                SingleTokenMode = true;
                Tokens.Add("1", value);
            }

            public Parameter(String key, String value) { Tokens.Add(key, value); }

            public Parameter(String key, Parameter value) { Tokens.Add(key, value); }

            public Parameter Add(String key, Parameter value)
            {
                if (SingleTokenMode)
                    throw new InvalidOperationException("Object is in single token mode.");

                Tokens.Add(key, value);
                return this;
            }

            public Parameter Add(String key, String value)
            {
                if (SingleTokenMode)
                    throw new InvalidOperationException("Object is in single token mode.");

                Tokens.Add(key, value);
                return this;
            }

            public String Get() => (String)Tokens["1"];

            public Object Get(String key) => Tokens[key];

            public Object TryGet(String key) => Tokens.ContainsKey(key) ? Tokens[key] : null;

            public Boolean Contains(String key) => Tokens.ContainsKey(key);

            public override String ToString() => AsObject().Stringify();

            public static Parameter Parse(Object parameter)
            {
                if (!(parameter is String) || String.IsNullOrWhiteSpace((String)parameter))
                    return null;

                var instance = new Parameter();
                instance.Instantiate(JsonObject.Parse((String)parameter));
                return instance;
            }

            private void Instantiate(JsonObject jsonObject)
            {
                foreach (var property in jsonObject)
                {
                    if (property.Value.ValueType == JsonValueType.Object)
                    {
                        var instance = new Parameter();
                        instance.Instantiate(property.Value.GetObject());
                        Tokens.Add(property.Key, instance);
                    }
                    else if (property.Value.ValueType == JsonValueType.String)
                    {
                        Tokens.Add(property.Key, property.Value.GetString());
                    }
                }
                SingleTokenMode = (Tokens.Count == 1) && Tokens.ContainsKey("1");
            }

            private JsonObject AsObject()
            {
                var jsonObject = new JsonObject();
                foreach (var token in Tokens)
                {
                    if (token.Value is String)
                        jsonObject.Add(token.Key, JsonValue.CreateStringValue((String)token.Value));
                    else if (token.Value is Parameter)
                        jsonObject.Add(token.Key, ((Parameter)token.Value).AsObject());
                }
                return jsonObject;
            }

            public IEnumerator<KeyValuePair<String, Object>> GetEnumerator() => Tokens.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => Tokens.GetEnumerator();
        }
    }
}
