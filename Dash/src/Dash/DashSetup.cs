using System;
using System.Collections.Generic;
using Blockcore.Consensus.Checkpoints;
using Blockcore.P2P;
using Dash.Networks;
using Dash.Networks.Setup;
using NBitcoin;

namespace Dash
{
   internal class DashSetup
   {
      internal static DashSetup Instance = new DashSetup();

      internal CoinSetup Setup = new CoinSetup
      {
         FileNamePrefix = "dash",
         ConfigFileName = "dash.conf",
         Magic = "BD-6B-0C-BF",
         CoinType = 5, // SLIP-0044: https://github.com/satoshilabs/slips/blob/master/slip-0044.md,
         PremineReward = 0,
         PoWBlockReward = 16,
         PoSBlockReward = 0,
         LastPowBlock = 0,
         GenesisText = "Wired 09/Jan/2014 The Grand Experiment Goes Live: Overstock.com Is Now Accepting Bitcoins", // The New York Times, 2020-04-16
         TargetSpacing = TimeSpan.FromSeconds(2.5 * 60),
         ProofOfStakeTimestampMask = 0, // 0x0000003F // 64 sec
         PoSVersion = 0
      };

      internal NetworkSetup Main = new NetworkSetup
      {
         Name = "DashMain",
         RootFolderName = "dash",
         CoinTicker = "Dash",
         DefaultPort = 9999,
         DefaultRPCPort = 9998,
         DefaultAPIPort = 9997,
         PubKeyAddress = 76, // B https://en.bitcoin.it/wiki/List_of_address_prefixes
         ScriptAddress = 16, // b
         SecretAddress = 204,
         GenesisTime = 1538481600,
         GenesisNonce = 1626464,
         GenesisBits = 0x1E0FFFFF,
         GenesisVersion = 1,
         GenesisReward = Money.Zero,
         HashGenesisBlock = "00000b0517068e602ed5279c20168cfa1e69884ee4e784909652da34c361bff2",
         HashMerkleRoot = "b3425d46594a954b141898c7eebe369c6e6a35d2dab393c1f495504d2147883b",
         DNS = new[] { "dnsseed.dash.org", "dnsseed.dashdot.io", "dnsseed.masternode.io", "dnsseed.dashpay.io"},
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         //Checkpoints = new Dictionary<int, CheckpointInfo>
         //{
         //}
      };

      internal NetworkSetup RegTest = new NetworkSetup
      {
         Name = "DashRegTest",
         RootFolderName = "dashregtest",
         CoinTicker = "TDash",
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
         DNS = new[] { "seedregtest1.dash.blockcore.net", "seedregtest2.dash.blockcore.net", "seedregtest.dash.blockcore.net" },
         Nodes = new[] { "10.10.10.10", "11.11.11.11" },
         //Checkpoints = new Dictionary<int, CheckpointInfo>
         //{
         //   // TODO: Add checkpoints as the network progresses.
         //}
      };

      internal NetworkSetup Test = new NetworkSetup
      {
         Name = "DashTest",
         RootFolderName = "dashtest",
         CoinTicker = "TDash",
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
         DNS = new[] { "seedtest1.dash.blockcore.net", "seedtest2.dash.blockcore.net", "seedtest.dash.blockcore.net" },
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
