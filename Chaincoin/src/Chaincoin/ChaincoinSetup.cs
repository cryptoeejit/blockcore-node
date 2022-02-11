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
      internal static ChaincoinSetup Instance = new ChaincoinSetup();

      internal CoinSetup Setup = new CoinSetup
      {
         FileNamePrefix = "chaincoin",
         ConfigFileName = "chaincoin.conf",
         Magic = "A3-D2-7A-03",
         CoinType = 711, // SLIP-0044: https://github.com/satoshilabs/slips/blob/master/slip-0044.md,
         PremineReward = 0,
         PoWBlockReward = 16,
         PoSBlockReward = 0,
         LastPowBlock = default(int),
         GenesisText = "18-01-14 - Anti-fracking campaigners chain themselves to petrol pumps", // The New York Times, 2020-04-16
         TargetSpacing = TimeSpan.FromSeconds(90),
         ProofOfStakeTimestampMask = 0x0000000F, // 0x0000003F // 64 sec
         PoSVersion = 4
      };

      internal NetworkSetup Main = new NetworkSetup
      {
         Name = "ChaincoinMain",
         RootFolderName = "chaincoin",
         CoinTicker = "CHC",
         DefaultPort = 11994,
         DefaultRPCPort = 11995,
         DefaultAPIPort = 11996,
         PubKeyAddress = 28, // B https://en.bitcoin.it/wiki/List_of_address_prefixes
         ScriptAddress = 4, // b
         SecretAddress = 156,
         GenesisTime = 1390078220,
         GenesisNonce = 2099366979,
         GenesisBits = 0x1E0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Coins(16m),
         HashGenesisBlock = "00000f639db5734b2b861ef8dbccc33aebd7de44d13de000a12d093bcc866c64",
         HashMerkleRoot = "fa6ef9872494fa9662cf0fecf8c0135a6932e76d7a8764e1155207f3205c7c88",
         DNS = new[] { "seed1.chaincoin.org", "seed2.chaincoin.org", "chc.seed.blockcore.net" },
         Nodes = new[] { "80.210.127.4", "195.201.19.94" },
         Checkpoints = new Dictionary<int, CheckpointInfo>
         {
               { 0, new CheckpointInfo(new uint256("0x00000f639db5734b2b861ef8dbccc33aebd7de44d13de000a12d093bcc866c64"))},
               { 6143, new CheckpointInfo(new uint256("0x0000000026fb51f5bc9943ed69d9ff7697ecf7fed419d88b417655f93a487ce1"))},
               { 12797, new CheckpointInfo(new uint256("0x000000002c29644e179baa188fa6b9b9454721f1f21f2b9f31eebe9acc1a31db"))},
               { 30092, new CheckpointInfo(new uint256("0x0000000098a23e1c503f71a6d61c333c5abaabb4c5fa1b474012e004db4bfbbe"))},
               { 80998, new CheckpointInfo(new uint256("0x000000010ebcfe9a00a99f2b61104f4a141555a707f1c007aba8a978f6030cfb"))},
               { 144759, new CheckpointInfo(new uint256("0x000000047e7b7bfd63b4f019a0a24c8d65b10afa6eb80721e10fa7c49ce6fb6e"))},
               { 189046, new CheckpointInfo(new uint256("0x00000000bd507c435b46ee8a13b25b85ec38fdb0eb5b00faeaa0611cd6a483d3"))},
               { 277316, new CheckpointInfo(new uint256("0x00000016a20503fe496e79d34fb85c33f633059315c046ffa1b4826d08a1e856"))},
               { 483849, new CheckpointInfo(new uint256("0x000001eb7f8124282ab62296e63d3145ff6c84cf18afae4d4b8e02cd3182b6a8"))},
               { 1066428, new CheckpointInfo(new uint256("0x000000012dc5256d977b50270d1ca5642726308dcf26b6c219985edb8f2ab8f6"))},
               { 1300730, new CheckpointInfo(new uint256("0x0000000001fdf11c0b4238b448c9a9643c7862575124fe0d7ee6fe7b5e7dba30"))},
               { 1384154, new CheckpointInfo(new uint256("0x0000000000fb3c41fb8a955b3c9fca128e57e51834347ea368adbea309fcd265"))},
         }
      };

      internal NetworkSetup RegTest = new NetworkSetup
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
         GenesisTime = 1587115302,
         GenesisNonce = 5917,
         GenesisBits = 0x1F00FFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "000039df5f7c79084bf96c67ea24761e177d77c24f326eb5294860144301cb68",
         HashMerkleRoot = "d382311c9e4a1ec84be1b32eddb33f7f0420544a460754f573d7cb7054566d75",
         DNS = new[] { "seedregtest1.chc.blockcore.net", "seedregtest2.chc.blockcore.net", "seedregtest.chc.blockcore.net" },
         Nodes = new[] { "80.210.127.4", "195.201.19.94" },
         //Checkpoints = new Dictionary<int, CheckpointInfo>
         //{
         //   // TODO: Add checkpoints as the network progresses.
         //}
      };

      internal NetworkSetup Test = new NetworkSetup
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
         GenesisTime = 1587115303,
         GenesisNonce = 3451,
         GenesisBits = 0x1F0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "00090058f8a37e4190aa341ab9605d74b282f0c80983a676ac44b0689be0fae1",
         HashMerkleRoot = "88cd7db112380c4d6d4609372b04cdd56c4f82979b7c3bf8c8a764f19859961f",
         DNS = new[] { "seedtest1.chc.blockcore.net", "seedtest2.chc.blockcore.net", "seedtest.chc.blockcore.net" },
         Nodes = new[] { "80.210.127.4", "195.201.19.94" },
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

   internal class OtherTracking
   {
      internal static int LastVersion = 1;
   }

}
