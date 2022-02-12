using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HashLib;
using NBitcoin;

namespace Dash.Networks.Crypto
{
   public sealed class HashX11
   {
      private readonly List<IHash> hashers;

      private readonly object hashLock;

      private static readonly Lazy<HashX11> SingletonInstance = new Lazy<HashX11>(LazyThreadSafetyMode.PublicationOnly);
      public HashX11()
      {
         hashers = new List<IHash>
         {
            HashFactory.Crypto.SHA3.CreateBlake512(),
            HashFactory.Crypto.SHA3.CreateBlueMidnightWish512(),
            HashFactory.Crypto.SHA3.CreateGroestl512(),
            HashFactory.Crypto.SHA3.CreateSkein512_Custom(),
            HashFactory.Crypto.SHA3.CreateJH512(),
            HashFactory.Crypto.SHA3.CreateKeccak512(),
            HashFactory.Crypto.SHA3.CreateLuffa512(),
            HashFactory.Crypto.SHA3.CreateCubeHash512(),
            HashFactory.Crypto.SHA3.CreateSHAvite3_512_Custom(),
            HashFactory.Crypto.SHA3.CreateSIMD512(),
            HashFactory.Crypto.SHA3.CreateEcho512(),
         };

         hashLock = new object();
         Multiplier = 1;
      }

      public uint Multiplier { get; private set; }

      /// <summary>
      /// using the instance method is not thread safe. 
      /// to calling the hashing method in a multi threaded environment use the create() method
      /// </summary>
      public static HashX11 Instance => SingletonInstance.Value;

      public static HashX11 Create()
      {
         return new HashX11();
      }

      public uint256 Hash(byte[] input)
      {
         byte[] buffer = input;

         lock (hashLock)
         {
            foreach (IHash hasher in hashers)
            {
               buffer = hasher.ComputeBytes(buffer).GetBytes();
            }
         }

         return new uint256(buffer.Take(32).ToArray());
      }
   }
}
