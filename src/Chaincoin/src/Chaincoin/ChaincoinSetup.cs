using System;
using System.Collections.Generic;
using Blockcore.Consensus.Checkpoints;
using Chaincoin.Networks;
using Chaincoin.Networks.Setup;
using NBitcoin;

namespace Chaincoin
{
   internal class ChaincoinSetup
   {
      internal static ChaincoinSetup Instance = new();

      internal CoinSetup Setup = new()
      {
         FileNamePrefix = "chaincoin",
         ConfigFileName = "chaincoin.conf",
         Magic = "A3-D2-7A-A1",
         CoinType = 711, // SLIP-0044: https://github.com/satoshilabs/slips/blob/master/slip-0044.md,
         PremineReward = 200000,
         PoWBlockReward = 10,
         PoSBlockReward = 16,
         LastPowBlock = 200,
         GenesisText = "The Sunday Times 06/Mar/2022 PMs Russian crony got peerage after spies dropped warning", // The New York Times, 2020-04-16
         TargetSpacing = TimeSpan.FromSeconds(64),
         ProofOfStakeTimestampMask = 0x0000000F, // 0x0000003F // 64 sec
         PoSVersion = 4
      };

      internal NetworkSetup Main = new()
      {
         Name = "ChaincoinMain",
         RootFolderName = "chaincoin",
         CoinTicker = "CHC",
         DefaultPort = 11884,
         DefaultRPCPort = 11885,
         DefaultAPIPort = 11886,
         PubKeyAddress = 28, // B https://en.bitcoin.it/wiki/List_of_address_prefixes
         ScriptAddress = 4, // b
         SecretAddress = 156,
         GenesisTime = 1647076204,
         GenesisNonce = 109484,
         GenesisBits = 0x1E0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "00000be69ac882231772eaf5f3f0fffe211c0f41fcf84adcafb90ff3de1ba725",
         HashMerkleRoot = "3c28b6da08f02e3d257cde878a1528c37aa5a073c9af9cf8b52512d018bb7752",
         DNS = new[] { "seed.yourdomain.com", "seed.secondomain.com" },
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         Checkpoints = new Dictionary<int, CheckpointInfo>
         {
            // TODO: Add checkpoints as the network progresses.
         }
      };

      internal NetworkSetup RegTest = new()
      {
         Name = "ChaincoinRegTest",
         RootFolderName = "chaincoinregtest",
         CoinTicker = "TCHC",
         DefaultPort = 9333,
         DefaultRPCPort = 9332,
         DefaultAPIPort = 9331,
         PubKeyAddress = 111,
         ScriptAddress = 196,
         SecretAddress = 239,
         GenesisTime = 1647076215,
         GenesisNonce = 63668,
         GenesisBits = 0x1F00FFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "000025e53a9a75a7170309c94d8106ed32a58d4e8fa8d405e8840cc53d8b4234",
         HashMerkleRoot = "fd1d5457e4dd7941dc816d865505785a06767240c2ea2262d58a7dc94c7f2ef5",
         DNS = new[] { "seedregtest1.chc.blockcore.net", "seedregtest2.chc.blockcore.net", "seedregtest.chc.blockcore.net" },
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         Checkpoints = new Dictionary<int, CheckpointInfo>
         {
            // TODO: Add checkpoints as the network progresses.
         }
      };

      internal NetworkSetup Test = new()
      {
         Name = "ChaincoinTest",
         RootFolderName = "chaincointest",
         CoinTicker = "TCHC",
         DefaultPort = 9333,
         DefaultRPCPort = 9332,
         DefaultAPIPort = 9331,
         PubKeyAddress = 111,
         ScriptAddress = 196,
         SecretAddress = 239,
         GenesisTime = 1646538621,
         GenesisNonce = 749,
         GenesisBits = 0x1F0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "000509b16943545352fdf2d81887b5f27002a2183994c426f9c496bcb453c03d",
         HashMerkleRoot = "714852e285b7a449f4462e30e8e966802c1fcb88654648035d3ea1173cd4b623",
         DNS = new[] { "seedtest1.chc.blockcore.net", "seedtest2.chc.blockcore.net", "seedtest.chc.blockcore.net" },
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         Checkpoints = new Dictionary<int, CheckpointInfo>
         {
            // TODO: Add checkpoints as the network progresses.
         }
      };

      public bool IsPoSv3()
      {
         return Setup.PoSVersion == 3;
      }

      public bool IsPoSv4()
      {
         return Setup.PoSVersion == 4;
      }
   }
}
