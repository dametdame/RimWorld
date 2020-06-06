// Log.cs
// Copyright Karel Kroeze, 2018-2020

using System.Diagnostics;

namespace DRimEditor.Research
{
    public static class Log
    {

        public static void Warning( string msg, params object[] args )
        {
            Verse.Log.Warning( Format( msg, args ) );
        }

        private static string Format( string msg, params object[] args )
        {
            return "DRimEditor.Research :: " + string.Format( msg, args );
        }

        public static void Error( string msg, bool once, params object[] args )
        {
            var _msg = Format( msg, args );
            if ( once )
                Verse.Log.ErrorOnce( _msg, _msg.GetHashCode() );
            else
                Verse.Log.Error( _msg );
        }
    }
}
