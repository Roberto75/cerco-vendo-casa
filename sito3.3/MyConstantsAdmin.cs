using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class MyConstantsAdmin
{
    public enum Esito
    {
        Ok,
        Errore
    }


    public static System.Globalization.CultureInfo CultureInfoEN = new System.Globalization.CultureInfo("en-GB");

    public static MyManagerCSharp.ManagerDB.Days DefaultDay = MyManagerCSharp.ManagerDB.Days.Anno_corrente;

    public const string TABLE_STYLE = "MyTable ui-responsive";
    public const string TABLE_ROW_STYLE = "ui-body-a";
    public const string TABLE_ALTERNATING_ROW_STYLE = "ui-body-g";
    public const string TABLE_HEADER_STYLE = "ui-bar-b";
    public const string TABLE_FOOTER_STYLE = "ui-bar-b";

    //public const string FIXED_FOOTER_THEME = "b";

    public const string UI_BLOCK_HEADER = "ui-bar ui-bar-a";
    public const string UI_BLOCK_BODY = "ui-bar ui-bar-c";

    public const string TAB_NAVBAR_BUTTON_THEME = "e";

    public const string BUTTON_THEME_UI = "ui-btn-b";
    public const string BUTTON_THEME = "b";
}
