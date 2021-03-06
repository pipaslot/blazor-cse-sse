﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Core.Localization;

namespace App.Server.Services
{
    public class ResourceManagerServer : ResourceManagerWithCulture
    {
        public ResourceManagerServer(Type classType) : base(classType.FullName, classType.Assembly)
        {
        }

        private CultureInfo? _culture;
        protected override Task OnCultureChanged(string culture)
        {
            _culture = new CultureInfo(culture);
            return Task.CompletedTask;
        }

        public override string GetString(string name, CultureInfo? culture)
        {
            return GetString(name);// TODO check if culture is really not needed
        }

        public override string GetString(string name)
        {
            var @string = _culture == null ? base.GetString(name) : base.GetString(name, _culture);
            return @string ?? "";
        }
    }
}