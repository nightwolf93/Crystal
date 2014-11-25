using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Forward.i18n;

namespace Crystal.WorldServer.Globalization
{
    public static class I18nManager
    {
        public static void LoadLangs()
        {
            new Loader(LanguageEnum.FR, "I18n/French.xml");
        }

        public static string GetText(int id, params string[] parameters)
        {
            return Loader.GetLoader(LanguageEnum.FR).GetText(id, parameters);
        }
    }
}
