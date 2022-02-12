using Blockcore.Networks;

namespace Dash.Networks
{
   public static class Networks
   {
      public static NetworksSelector Dash
      {
         get
         {
            return new NetworksSelector(() => new DashMain(), () => new DashTest(), () => new DashRegTest());
         }
      }
   }
}
