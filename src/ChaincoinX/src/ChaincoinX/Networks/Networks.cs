using Blockcore.Networks;

namespace ChaincoinX.Networks
{
   public static class Networks
   {
      public static NetworksSelector ChaincoinX
      {
         get
         {
            return new NetworksSelector(() => new ChaincoinXMain(), () => new ChaincoinXTest(), () => new ChaincoinXRegTest());
         }
      }
   }
}
