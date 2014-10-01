using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearch
{
    public class JsonDataConveter
    {
        public void Test()
        {
            string json = @"	{
	    ""AddItemsToIndexRequest"": {
	        ""AuthToken"": ""${AuthToken}"",
			""TestingFlag"": ${TestingFlag},
			""TestingErrorCode"": ""${TestingErrorCode}"",
			""RequestID"": ""${RequestID}"",
	        ""ItemList"":
			[
				{
					""VideoTitle"":""${VideoTitle}"",
					""Actors"": [""Actor1"",""Actor2""],
					""PublishedDate"": ""${PublishedDate}"",
					""Publisher"": ""${Publisher}"",
					""ImgLink"": ""${ImgLink}"", /*not index*/
					""ThumbImgLink"": ""${ThumbImgLink}"" /*not index*/
					/*""Tags"": [""Tag1"",""Tag2""]*/	
				},
				{
					""VideoTitle"":""${VideoTitle}"",
					""Actors"": [""Actor1"",""Actor2""],
					""PublishedDate"": """"
				}		
			]      
	    }
	}";

            JObject jsonObject = JsonConvert.DeserializeObject(json) as JObject;
            Console.WriteLine("Value:" + jsonObject.ToString());
            Console.WriteLine("Type:" + jsonObject.GetType().FullName);
            string email = jsonObject["Email"].Value<string>();
            string strDate = jsonObject["CreatedDate"].Value<string>();
            DateTime dtDate = jsonObject["CreatedDate"].Value<DateTime>();
            string strActive = jsonObject["Active"].Value<string>();
            Console.WriteLine(email);
            Console.WriteLine(strDate);
            Console.WriteLine(dtDate);
            Console.WriteLine(strActive);

            //要轉換的目標型別
            string strInputType = "System.Int32";
            //要被轉換的字串
            string strInputValue = "123";
            Type type = Type.GetType(strInputType);
            object targetValue = Convert.ChangeType(strInputValue, type);
            Type targetType = targetValue.GetType();
            Console.WriteLine("Result: {0} ", targetValue);
            Console.WriteLine("Type: {0} ", targetType.FullName);
        }
    }
}
