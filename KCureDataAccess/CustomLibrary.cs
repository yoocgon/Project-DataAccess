using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCureDataAccess
{
    public  class CustomLibrary
    {

        public string CamelToSnake(string camelCase)
        {
            return string.Concat(camelCase.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public List<Dictionary<string, object>> SnakeKeyToCamelKey(List<Dictionary<string, object>> listDicData)
        {
            List<Dictionary<string, object>> listDicCamelKey = new List<Dictionary<string, object>>();
            //
            if (listDicData != null)
            {
                foreach (var dic in listDicData)
                {
                    var newDic = new Dictionary<string, object>();
                    foreach (var key in dic.Keys)
                    {
                        var newKey = string.Concat(key.Split('_').Select((x, i) => i > 0 ? char.ToUpper(x[0]) + x.Substring(1) : x));
                        newDic.Add(newKey, dic[key]);
                    }
                    // listDicData[listDicData.IndexOf(dic)] = newDic;
                    listDicCamelKey.Add(newDic);
                }
            }
            //
            return listDicCamelKey;
        }

    }
}
