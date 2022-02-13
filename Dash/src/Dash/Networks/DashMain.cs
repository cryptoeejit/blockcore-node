using System;
using System.Collections.Generic;
using Dash.Networks.Policies;
using Dash.Networks.Rules;
using System.Linq;
using System.Net;
using Dash.Networks.Setup;
using Dash.Networks.Deployments;
using Blockcore.Networks;
using NBitcoin;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus;
using Blockcore.Base.Deployments;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using Blockcore.P2P;
using Blockcore.Features.Consensus.Rules.CommonRules;
using Blockcore.Features.Consensus.Rules.ProvenHeaderRules;
using Blockcore.Features.Consensus.Rules.UtxosetRules;
using Blockcore.Features.MemoryPool.Rules;
using Blockcore.Consensus.TransactionInfo;
using Blockcore.Consensus.ScriptInfo;

namespace Dash.Networks
{
   public class DashMain : Network
   {
      public DashMain()
      {
         CoinSetup setup = DashSetup.Instance.Setup;
         NetworkSetup network = DashSetup.Instance.Main;

         NetworkType = NetworkType.Mainnet;
         DefaultConfigFilename = setup.ConfigFileName; // The default name used for the Dash configuration file.

         Name = network.Name;
         CoinTicker = network.CoinTicker;
         Magic = ConversionTools.ConvertToUInt32(setup.Magic);
         RootFolderName = network.RootFolderName;
         DefaultPort = network.DefaultPort;
         DefaultRPCPort = network.DefaultRPCPort;
         DefaultAPIPort = network.DefaultAPIPort;

         DefaultMaxOutboundConnections = 16;
         DefaultMaxInboundConnections = 109;
         MaxTipAge = 2 * 60 * 60;
         MinTxFee = 10000;
         MaxTxFee = Money.Coins(1).Satoshi;
         FallbackFee = 0;
         MinRelayTxFee = 10000;
         MaxTimeOffsetSeconds = 25 * 60;
         DefaultBanTimeSeconds = 16000; // 500 (MaxReorg) * 64 (TargetSpacing) / 2 = 4 hours, 26 minutes and 40 seconds

         var consensusFactory = new PosConsensusFactory();

         // Create the genesis block.
         GenesisTime = network.GenesisTime;
         GenesisNonce = network.GenesisNonce;
         GenesisBits = network.GenesisBits;
         GenesisVersion = network.GenesisVersion;
         GenesisReward = network.GenesisReward;

         Block genesisBlock = CreateGenesisBlock(consensusFactory,
            GenesisTime,
            GenesisNonce,
            GenesisBits,
            GenesisVersion,
            GenesisReward,
            setup.GenesisText);

         Genesis = genesisBlock;

         var consensusOptions = new PosConsensusOptions
         {
            MaxBlockBaseSize = 1_000_000,
            MaxStandardVersion = 2,
            MaxStandardTxWeight = 100_000,
            MaxBlockSigopsCost = 20_000,
            MaxStandardTxSigopsCost = 20_000 / 5,
            WitnessScaleFactor = 4
         };

         //https://github.com/dashpay/dash/blob/master/src/chainparams.cpp
         //consensus.BIP34Height = 951;
         //consensus.BIP34Hash = uint256S("0x000001f35e70f7c5705f64c6c5cc3dea9449e74d5b5c7cf74dad1bcca14a8012");
         //consensus.BIP65Height = 619382; // 00000000000076d8fcea02ec0963de4abfd01e771fec0863f960c2c64fe6f357
         //consensus.BIP66Height = 245817; // 00000000000b1fa2dfa312863570e13fae9ca7b5566cb27e55422620b469aefa
         //consensus.DIP0001Height = 782208;
         //consensus.DIP0003Height = 1028160;
         //consensus.DIP0003EnforcementHeight = 1047200;
         //consensus.DIP0003EnforcementHash = uint256S("000000000000002d1734087b4c5afc3133e4e1c3e1a89218f62bcd9bb3d17f81");
         //consensus.DIP0008Height = 1088640; // 00000000000000112e41e4b3afda8b233b8cc07c532d2eac5de097b68358c43e

         var buriedDeployments = new BuriedDeploymentsArray
         {
            [BuriedDeployments.BIP34] = 951,
            [BuriedDeployments.BIP65] = 619382,
            [BuriedDeployments.BIP66] = 245817
         };

         var bip9Deployments = new DashBIP9Deployments()
         {
            [DashBIP9Deployments.TestDummy] = new BIP9DeploymentsParameters("TestDummy", 28, 1199145601, 1230767999, BIP9DeploymentsParameters.DefaultMainnetThreshold),
            [DashBIP9Deployments.CSV] = new BIP9DeploymentsParameters("CSV", 0, 1486252800, 1517788800, BIP9DeploymentsParameters.DefaultMainnetThreshold),
            //[DashBIP9Deployments.Segwit] = new BIP9DeploymentsParameters("Segwit", 1, 1527811200, 1577750400, BIP9DeploymentsParameters.DefaultMainnetThreshold)
         };

         consensusFactory.Protocol = new ConsensusProtocol()
         {
            ProtocolVersion = 70219,
            MinProtocolVersion = 70219,
         };

         Consensus = new Blockcore.Consensus.Consensus(
             consensusFactory: consensusFactory,
             consensusOptions: consensusOptions,
             coinType: setup.CoinType,
             hashGenesisBlock: genesisBlock.GetHash(),
             subsidyHalvingInterval: 210240,
             majorityEnforceBlockUpgrade: 750,
             majorityRejectBlockOutdated: 950,
             majorityWindow: 1000,
             buriedDeployments: buriedDeployments,
             bip9Deployments: bip9Deployments,
             bip34Hash: new uint256("0x000007d91d1254d60e2dd1ae580383070a4ddffa4c64c2eeb4a2f9ecc0414343"),
             //ruleChangeActivationThreshold: 1916, Blockcore?
             minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
             maxReorgLength: 500,
             defaultAssumeValid: null,
             maxMoney: long.MaxValue,
             coinbaseMaturity: 100,
             premineHeight: 0,
             premineReward: Money.Coins(setup.PremineReward),
             proofOfWorkReward: Money.Coins(setup.PoWBlockReward),
             targetTimespan: TimeSpan.FromSeconds(24 * 60 * 60), 
             targetSpacing: setup.TargetSpacing,
             powAllowMinDifficultyBlocks: false,
             posNoRetargeting: false,
             powNoRetargeting: false,
             powLimit: new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
             minimumChainWork: null,
             isProofOfStake: false,
             lastPowBlock: setup.LastPowBlock,
             proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
             proofOfStakeLimitV2: new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
             proofOfStakeReward: Money.Coins(setup.PoSBlockReward),
             proofOfStakeTimestampMask: setup.ProofOfStakeTimestampMask
         )
         {
            PosEmptyCoinbase = DashSetup.Instance.IsPoSv3(),
            PosUseTimeFieldInKernalHash = DashSetup.Instance.IsPoSv3()
         };

         // TODO: Set your Base58Prefixes
         Base58Prefixes = new byte[12][];
         Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (byte)network.PubKeyAddress };
         Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (byte)network.ScriptAddress };
         Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (byte)network.SecretAddress };

         Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
         Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
         Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
         Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
         Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
         Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
         Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 }; // ?

         Bech32Encoders = new Bech32Encoder[2];
         var encoder = new Bech32Encoder(network.CoinTicker.ToLowerInvariant());
         Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
         Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

         Checkpoints = network.Checkpoints;
         DNSSeeds = network.DNS.Select(dns => new DNSSeedData(dns, dns)).ToList();
         SeedNodes = network.Nodes.Select(node => new NBitcoin.Protocol.NetworkAddress(IPAddress.Parse(node), network.DefaultPort)).ToList();

         StandardScriptsRegistry = new DashStandardScriptsRegistry();

      //assert(consensus.hashGenesisBlock == uint256S("0x00000ffd590b1485b3caadc19b22e6379c733355108f107a430458cdf3407ab6"));
      //assert(genesis.hashMerkleRoot == uint256S("0xe0028eb9648db56b1ac77cf090b99048a8007e2bb64b68f092c03c7f56a662c7"));
         Assert(Consensus.HashGenesisBlock == uint256.Parse(network.HashGenesisBlock));   
         Assert(Genesis.Header.HashMerkleRoot == uint256.Parse(network.HashMerkleRoot));

         RegisterRules(Consensus);
         RegisterMempoolRules(Consensus);
      }

      protected void RegisterRules(IConsensus consensus)
      {
         consensus.ConsensusRules
           .Register<HeaderTimeChecksRule>()
           //.Register<CheckDifficultyPowRule>()
           //.Register<DashActivationRule>()
           .Register<DashHeaderVersionRule>();

         consensus.ConsensusRules
             .Register<BlockMerkleRootRule>();

         consensus.ConsensusRules
             .Register<SetActivationDeploymentsPartialValidationRule>()

             .Register<TransactionLocktimeActivationRule>() // implements BIP113
             .Register<CoinbaseHeightActivationRule>() // implements BIP34
             .Register<WitnessCommitmentsRule>() // BIP141, BIP144
             .Register<BlockSizeRule>()

             // rules that are inside the method CheckBlock
             .Register<EnsureCoinbaseRule>()
             .Register<CheckPowTransactionRule>()
             .Register<CheckSigOpsRule>();
      }

      protected void RegisterMempoolRules(IConsensus consensus)
      {
         consensus.MempoolRules = new List<Type>()
            {
                typeof(CheckConflictsMempoolRule),
                typeof(CheckCoinViewMempoolRule),
                typeof(CreateMempoolEntryMempoolRule),
                typeof(CheckSigOpsMempoolRule),
                typeof(CheckFeeMempoolRule),
                typeof(CheckRateLimitMempoolRule),
                typeof(CheckAncestorsMempoolRule),
                typeof(CheckReplacementMempoolRule),
                typeof(CheckAllInputsMempoolRule),
                typeof(CheckTxOutDustRule)
            };
      }

      protected static Block CreateGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward, string genesisText)
      {
         Transaction txNew = consensusFactory.CreateTransaction();
         txNew.Version = 1;

         if (txNew is IPosTransactionWithTime posTx)
         {
            posTx.Time = nTime;
         }

         txNew.AddInput(new TxIn()
         {
            ScriptSig = new Script(Op.GetPushOp(0), new Op()
            {
               Code = (OpcodeType)0x1,
               PushData = new[] { (byte)42 }
            }, Op.GetPushOp(Encoders.ASCII.DecodeData(genesisText)))
         });

         txNew.AddOutput(new TxOut()
         {
            Value = genesisReward,
         });

         Block genesis = consensusFactory.CreateBlock();
         genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
         genesis.Header.Bits = nBits;
         genesis.Header.Nonce = nNonce;
         genesis.Header.Version = nVersion;
         genesis.Transactions.Add(txNew);
         genesis.Header.HashPrevBlock = uint256.Zero;
         genesis.UpdateMerkleRoot();

         return genesis;
      }
   }
}
