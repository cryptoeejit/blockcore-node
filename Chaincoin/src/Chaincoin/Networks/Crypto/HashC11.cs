using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HashLib;
using NBitcoin;

namespace Chaincoin.Networks.Crypto
{
   public sealed class HashC11
   {
      private readonly List<IHash> hashers;

      private readonly object hashLock;

      private static readonly Lazy<HashC11> SingletonInstance = new Lazy<HashC11>(LazyThreadSafetyMode.PublicationOnly);
      public HashC11()
      {
         hashers = new List<IHash>
         {
            HashFactory.Crypto.SHA3.CreateBlake512(),
            HashFactory.Crypto.SHA3.CreateBlueMidnightWish512(),
            HashFactory.Crypto.SHA3.CreateGroestl512(),
            HashFactory.Crypto.SHA3.CreateJH512(),
            HashFactory.Crypto.SHA3.CreateKeccak512(),
            HashFactory.Crypto.SHA3.CreateSkein512_Custom(),
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
      public static HashC11 Instance => SingletonInstance.Value;

      public static HashC11 Create()
      {
         return new HashC11();
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
