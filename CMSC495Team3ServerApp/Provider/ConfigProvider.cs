﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using CMSC495Team3ServerApp.Logging;

namespace CMSC495Team3ServerApp.Provider
{
    public class ConfigProvider : IConfigProvider
    {
        private const string NOT_DEFINED = " was not defined in the environment variables.";
        private const string NOT_VALID = " does not have a valid value in the environment variables.";

        private readonly NameValueCollection configSection;
        private readonly ILogger log;

        private readonly List<ArgumentException> ValidationErrors;

        public ConfigProvider(ILogger log)
        {
            this.log = log;
            ValidationErrors = new List<ArgumentException>();

            this.log.Info("====================================================\n" +
                          "Reading and setting application configurations\n" +
                          "====================================================");

            configSection =
                (NameValueCollection) ConfigurationManager.GetSection("serverSettings");

            GetEnvironmentVariables();

            this.log.Info("----------------------------------------------------\n" +
                          "Finished loading configurations\n" +
                          "----------------------------------------------------");

            if (!ValidationErrors.Any()) return;

            ValidationErrors.ForEach(error => log.Fatal(error.Message));

            throw new AggregateException(ValidationErrors);
        }

        public string DatabaseConnectionString { get; private set; }

        public string ExposedHttpUrl { get; private set; }

        public string UntappdApiUrlBase { get; private set; }

        public string UntappdAppClientId { get; private set; }

        public string UntappdAppClientSecret { get; private set; }

        private void GetEnvironmentVariables()
        {
            ExposedHttpUrl = GetConfiguration<string>("ExposedHttpUrl");
            DatabaseConnectionString = GetConfiguration<string>("DatabaseConnectionString");
            UntappdApiUrlBase = GetConfiguration<string>("UntappdApiUrlBase");
            UntappdAppClientId = GetConfiguration<string>("UntappdAppClientId");
            UntappdAppClientSecret = GetConfiguration<string>("UntappdAppClientSecret");
        }

        private T GetConfiguration<T>(string keyName) where T : IConvertible
        {
            var value = configSection.Get(keyName);

            if (value != null)
            {
                log.Info($"'{keyName}' - '{value}'");
            }
            else
            {
                ValidationErrors.Add(new ArgumentException(keyName + NOT_DEFINED));

                return default(T);
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                return (T) converter.ConvertFromString(value);
            }
            catch (Exception)
            {
                ValidationErrors.Add(new ArgumentException(keyName + NOT_VALID));

                return default(T);
            }
        }

        public T GetVariableValue<T>(string sectionName)
        {
            return (T) ConfigurationManager.GetSection(sectionName);
        }

        private IEnumerable<T> GetConfiguration<T>(string keyName, char delimiter) where T : IConvertible
        {
            var value = Environment.GetEnvironmentVariable(keyName);

            if (value != null)
            {
                log.Info($"'{keyName}' - '{value}'");
            }
            else
            {
                ValidationErrors.Add(new ArgumentException(keyName + NOT_DEFINED));

                return default(IEnumerable<T>);
            }

            var values = value.Split(delimiter);

            var converter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                var convertedValues = values.Select(val => (T) converter.ConvertFromString(val)).ToList();

                return convertedValues;
            }
            catch (Exception)
            {
                ValidationErrors.Add(new ArgumentException(keyName + NOT_VALID));

                return default(IEnumerable<T>);
            }
        }
    }
}