using System;
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
        private readonly ILogger log;

        private const string NOT_DEFINED = " was not defined in the environment variables.";

        private const string NOT_VALID = " does not have a valid value in the environment variables.";

        public string DatabaseConnectionString { get; private set; }

        public string ExposedHttpUrl { get; private set; }

        private readonly List<ArgumentException> ValidationErrors;

        public ConfigProvider(ILogger log)
        {
            this.log = log;
            ValidationErrors = new List<ArgumentException>();

            log.Info("====================================================");
            log.Info("Reading and setting application configurations");
            log.Info("====================================================");

            GetEnvironmentVariables();

            log.Info("----------------------------------------------------");
            log.Info("Finished loading configurations");
            log.Info("----------------------------------------------------");


            if (!ValidationErrors.Any())
            {
                return;
            }

            ValidationErrors.ForEach(error => log.Fatal(error.Message));

            throw new AggregateException(ValidationErrors);
        }

        private void GetEnvironmentVariables()
        {
            ExposedHttpUrl = GetConfiguration<string>("ExposedHttpUrl");

            DatabaseConnectionString = GetConfiguration<string>("DatabaseConnectionString");

            //LocalRootFilePath = GetConfiguration<string>("LocalRootFilePath");

            //SessionDurationInMins = GetConfiguration<int>("SessionDurationInMins");
        }

        private T GetConfiguration<T>(string keyName) where T : IConvertible
        {
            var configSection =
                (NameValueCollection)ConfigurationManager.GetSection("serverSettings");

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
                return (T)converter.ConvertFromString(value);
            }
            catch (Exception)
            {
                ValidationErrors.Add(new ArgumentException(keyName + NOT_VALID));

                return default(T);
            }
        }

        public T GetVariableValue<T>(string sectionName)
        {
            return (T)ConfigurationManager.GetSection(sectionName);
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
                var convertedValues = values.Select(val => (T)converter.ConvertFromString(val)).ToList();

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