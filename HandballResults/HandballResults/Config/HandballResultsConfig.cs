﻿using System.Configuration;

namespace HandballResults.Config
{
    public class HandballResultsConfig
    {
        public static HandballResultsConfigsSection Config =
            ConfigurationManager.GetSection("handballResults") as HandballResultsConfigsSection;
        public static TeamsCollection GetExcludedTeams()
        {
            return Config.ExcludedTeams;
        }
    }

    public class HandballResultsConfigsSection : ConfigurationSection
    {
        [ConfigurationProperty("excludedTeams")]
        public TeamsCollection ExcludedTeams => (TeamsCollection)this["excludedTeams"];
    }

    [ConfigurationCollection(typeof(TeamElement), AddItemName = "team")]
    public class TeamsCollection : ConfigurationElementCollection
    {
        public TeamElement this[int index]
        {
            get => (TeamElement)BaseGet(index);
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new TeamElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TeamElement)element).Id;
        }
    }

    public class TeamElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public int Id
        {
            get => (int)this["id"];
            set => this["id"] = value;
        }
    }
}