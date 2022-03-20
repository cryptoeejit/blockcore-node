using System;
using System.Collections.Generic;
using Blockcore.Consensus.Checkpoints;
using ChaincoinX.Networks;
using ChaincoinX.Networks.Setup;
using NBitcoin;

namespace ChaincoinX
{
   internal class ChaincoinXSetup
   {
      internal static ChaincoinXSetup Instance = new ChaincoinXSetup();

      internal CoinSetup Setup = new CoinSetup
      {
         FileNamePrefix = "chaincoinx",
         ConfigFileName = "chaincoinx.conf",
         Magic = "01-52-53-44",
         CoinType = 7111, // SLIP-0044: https://github.com/satoshilabs/slips/blob/master/slip-0044.md,
         PremineReward = 20000000,
         PoWBlockReward = 2,
         PoSBlockReward = 2,
         LastPowBlock = 1000,
         MaxSupply = 23000000,
         GenesisText = "The Times 18/Mar/2022 Bank of England signals caution as it raises rates", // The New York Times, 2020-04-16
         TargetSpacing = TimeSpan.FromSeconds(64),
         ProofOfStakeTimestampMask = 0x0000000F, // 0x0000003F // 64 sec
         PoSVersion = 4
      };

      internal NetworkSetup Main = new NetworkSetup
      {
         Name = "ChaincoinXMain",
         RootFolderName = "chaincoinx",
         CoinTicker = "CHX",
         DefaultPort = 11884,
         DefaultRPCPort = 11885,
         DefaultAPIPort = 11886,
         PubKeyAddress = 28, // B https://en.bitcoin.it/wiki/List_of_address_prefixes
         ScriptAddress = 110, // b
         SecretAddress = 160,
         GenesisTime = 1647640410,
         GenesisNonce = 1199154,
         GenesisBits = 0x1E0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "00000888a119343ec12b50eca81cd5dd6574b234f761a11919e859a9ebd0a629",
         HashMerkleRoot = "221c31f00f510cd692f0762d456960fec548e79364dcd3af7549b698279e0582",
         DNS = new[] { "seed.yourdomain.com", "seed.secondomain.com", "chx.seed.blockcore.net" },
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         Checkpoints = new Dictionary<int, CheckpointInfo>
         {
            // TODO: Add checkpoints as the network progresses.
         }
      };

      internal NetworkSetup RegTest = new NetworkSetup
      {
         Name = "ChaincoinXRegTest",
         RootFolderName = "chaincoinxregtest",
         CoinTicker = "TCHX",
         DefaultPort = 9333,
         DefaultRPCPort = 9332,
         DefaultAPIPort = 9331,
         PubKeyAddress = 111,
         ScriptAddress = 196,
         SecretAddress = 239,
         GenesisTime = 1647143210,
         GenesisNonce = 46141,
         GenesisBits = 0x1F00FFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "000056fc8f24f1cc6bbb409dec476f398b7ac2fc767b361b91c9faebf7c882c0",
         HashMerkleRoot = "7752e261ac21c89172ed35c581ad30e45524eda23ad0457352c9109c19c1d7f7",
         DNS = new[] { "seedregtest1.chx.blockcore.net", "seedregtest2.chx.blockcore.net", "seedregtest.chx.blockcore.net" },
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         Checkpoints = new Dictionary<int, CheckpointInfo>
         {
            // TODO: Add checkpoints as the network progresses.
         }
      };

      internal NetworkSetup Test = new NetworkSetup
      {
         Name = "ChaincoinXTest",
         RootFolderName = "chaincoinxtest",
         CoinTicker = "TCHX",
         DefaultPort = 9333,
         DefaultRPCPort = 9332,
         DefaultAPIPort = 9331,
         PubKeyAddress = 111,
         ScriptAddress = 196,
         SecretAddress = 239,
         GenesisTime = 1647143214,
         GenesisNonce = 9448,
         GenesisBits = 0x1F0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "000d14dcd6fb7b679eb1700e1fe07f8f50b2b845b4424f8f5de8334d3c735ec8",
         HashMerkleRoot = "4844c14eb37d9239a0a675f89003b002026742d92f86e53bb9f83686337b9003",
         DNS = new[] { "seedtest1.chx.blockcore.net", "seedtest2.chx.blockcore.net", "seedtest.chx.blockcore.net" },
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
