using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

/*
    Ce fichier est une partie du projet Forward

    Forward est un logiciel libre : vous pouvez redistribué et/ou modifié
    les sources sous les thermes de la General Public License (GNU) publié par
    la Free Software Foundation, ou bien de la version 3 de la license ou tout
    autre version antérieur.
    
    Forward est distribué dans la volonté d'être utile SANS AUCUNE GARANTIE, 
	néanmoins en aucun cas il ne doit pas ETRE UTILISE OU DISTRIBUE A DES FINS COMMERCIALES,
    voir la General Public License (GNU) pour plus de détails.

    Un copie de la General Public License (GNU) est distribué avec le logiciel Forward
    En cas contraire visitez : <http://www.gnu.org/licenses/>.
  
    Forward Copyright (C) 2011 NightWolf — Tous droits réservés.
	Créé par NightWolf

*/

namespace Forward.i18n
{
    public class Loader
    {
        #region Static

        // Dictionnaire static, pour recuperer un loader charger
        public static Dictionary<LanguageEnum, Loader> Languages = new Dictionary<LanguageEnum, Loader>();

        public static Loader GetLoader(LanguageEnum lang)
        {
            return Languages[lang];
        }

        #endregion

        #region Instance

        public Dictionary<int, string> Texts = new Dictionary<int, string>();

        public Loader(LanguageEnum lang, string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Can't find i18n file !");
            }

            var i18nFile = new XmlDocument();
            i18nFile.Load(path);

            foreach (XmlNode mainNode in i18nFile.GetElementsByTagName("i18nText"))
            {
                var textId = int.Parse(mainNode.Attributes["id"].InnerText);
                var textStr = mainNode.InnerText;
                this.Texts.Add(textId, textStr);
            }

            if (!Languages.ContainsKey(lang))
            {
                Languages.Add(lang, this);
            }
            else
            {
                throw new Exception("The specificated language was already loaded in i18n cache !");
            }
        }

        public string GetText(int id)
        {
            if (!this.Texts.ContainsKey(id))
                return "null";

            return this.Texts[id];
        }

        public string GetText(int id, params string[] parameters)
        {
            if (!this.Texts.ContainsKey(id))
                return "null";

            string text = this.Texts[id];
            for (int i = 0; i <= parameters.Count() - 1; i++)
            {
                text = text.Replace("{" + i + "}", parameters[i]);
            }
            return text;
        }

        #endregion
    }
}
