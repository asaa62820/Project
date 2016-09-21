using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace UniversalAnalyse
{
    class spinner_format
    {
        public string spinner_clearformat(string Key)
        {
            string s = Key;

            string a = reg0.Replace(s, "");
            a = reg1.Replace(a, "");
            a = reg2.Replace(a, "");

            return a;
        }


        static Regex reg0 = new Regex(@"\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex reg1 = new Regex("【1】", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex reg2 = new Regex("w+([-+.]w+)*@w+([-.]w+)*.w+([-.]w+)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        /*
         * 字母n开头，以序列ion结尾的所有字 需要一个以\bn开头，以ion\b结尾的模式  String Pattern = @"\bn\S*ion\b";
         * 
            published by***
            Published By***
            published by***
            Source:***
            Author:***
            Website:***
            My website is***
            our web site***
            on our web site***
            by e-mailing us***
            sites:***
            Email me***
            I am located in***
            on our web site***
            you may contact at***
            My website***
            by emailing***
            my email address***
            welcome to call me***
            to email me***
            Email me at***
            whom you may contact at***
         
         * 
            [***]                    "\b[\S*]\b"
            http://***  <-
            http://***.htm
            http://***.jsp
            http://***.html
            http://***.net
            http://***.org
            http://***.com/
            visit: ***.com
            www.***.com
            www.***.org
            www.***.net
            http://www.***.NET.au
            http://www.***.com.au

            ***.net <-
            ***.com <-
            ***.org <-
            site on ***
            webmaster@reinventingmyself.com
            please visit***
            (647)898-6504
            sales@bestforbride.com
            Copyright***

            http://www.ujc.org/content_display.html?ArticleID=142336</blockquote>
            sites:travelwires.com/wp/?p=2083en.wikipedia.org/wiki/World_Heritage_Site
            For more information***
            For more info***
            http://***.aspx
            Post Office Box***
            I am located in***
            abc.123/***.html
            abc.123/***.htm
            abc.123/***.aspx
            abc.123/***.jsp
            abc.-123/***.php
            Copyright ***
         
         */


    }
}
