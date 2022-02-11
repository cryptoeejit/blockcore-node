using System;
using System.Collections.Generic;
using Blockcore.Features.Consensus.Rules.CommonRules;
using Blockcore.Features.Consensus.Rules.ProvenHeaderRules;
using Blockcore.Features.Consensus.Rules.UtxosetRules;
using Blockcore.Features.MemoryPool.Rules;
using Chaincoin.Networks.Policies;
using Chaincoin.Networks.Rules;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using System.Collections;
using System.Linq;
using System.Collections.Specialized;
using System.Net;
using Chaincoin.Networks.Setup;
using Blockcore.Networks;
using Blockcore.Base.Deployments;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus;
using Blockcore.P2P;
using Blockcore.Consensus.TransactionInfo;
using Blockcore.Consensus.ScriptInfo;
using Chaincoin.Networks.Deployments;
using Chaincoin.Networks.Consensus;
using Blockcore.Consensus.Checkpoints;
using Blockcore.Networks.Rules;

namespace Chaincoin.Networks
{
   public class ChaincoinMain : Network
   {
      public ChaincoinMain()
      {
         CoinSetup setup = ChaincoinSetup.Instance.Setup;
         NetworkSetup network = ChaincoinSetup.Instance.Main;

         NetworkType = NetworkType.Mainnet;
         DefaultConfigFilename = setup.ConfigFileName; // The default name used for the Chaincoin configuration file.

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
         FallbackFee = Money.Zero; 
         
         MinRelayTxFee = 1000;
         MaxTimeOffsetSeconds = 25 * 60;
         DefaultBanTimeSeconds = 16000; // 500 (MaxReorg) * 64 (TargetSpacing) / 2 = 4 hours, 26 minutes and 40 seconds

         var consensusFactory = new ChaincoinConsensusFactory();

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

         //string genesisBytes = "010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efa0ce9da52ffff0f1e43cc217d0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000";
         //Block genesisBlock = Block.Parse(genesisBytes, consensusFactory);
         //Block genesisBlock = Block.Load(genesisBytes, consensusFactory).Header.GetHash());

         Genesis = genesisBlock;

         var consensusOptions = new ConsensusOptions
         {
            MaxBlockBaseSize = 1_000_000,
            MaxStandardVersion = 2,
            MaxStandardTxWeight = 100_000,
            MaxBlockSigopsCost = 20_000,
            MaxStandardTxSigopsCost = 20_000 / 5,
            WitnessScaleFactor = 4
         };

         var buriedDeployments = new BuriedDeploymentsArray
         {
            [BuriedDeployments.BIP34] = 1,
            [BuriedDeployments.BIP65] = 1551170,
            [BuriedDeployments.BIP66] = 1000000
         };

         var bip9Deployments = new ChaincoinBIP9Deployments()
         {
            [ChaincoinBIP9Deployments.TestDummy] = new BIP9DeploymentsParameters("TestDummy", 28, 1199145601, 1230767999, BIP9DeploymentsParameters.DefaultMainnetThreshold),
            [ChaincoinBIP9Deployments.CSV] = new BIP9DeploymentsParameters("CSV", 0, 1527811200, 1577750400, BIP9DeploymentsParameters.DefaultMainnetThreshold),
            [ChaincoinBIP9Deployments.Segwit] = new BIP9DeploymentsParameters("Segwit", 1, 1527811200, 1577750400, BIP9DeploymentsParameters.DefaultMainnetThreshold)
         };

         consensusFactory.Protocol = new ConsensusProtocol()
         {
            ProtocolVersion = 70015,
            MinProtocolVersion = 70015,
         };

         Consensus = new ChaincoinConsensus(
            consensusFactory: consensusFactory,
            consensusOptions: consensusOptions,
            coinType: setup.CoinType,
            hashGenesisBlock: genesisBlock.GetHash(),
            subsidyHalvingInterval: 210000,
            majorityEnforceBlockUpgrade: 750,
            majorityRejectBlockOutdated: 950,
            majorityWindow: 1000,
            buriedDeployments: buriedDeployments,
            bip9Deployments: bip9Deployments,
            bip34Hash: new uint256("0x00000012f1c40ff12a9e6b0e9076fe4fa7ad27012e256a5ad7bcb80dc02c0409"),
            //minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
            //ruleChangeActivationThreshold: 10752, // 80% of 13440
            minerConfirmationWindow: 13440, // average of 2 weeks
            powAllowMinDifficultyBlocks: false,

            maxReorgLength: 500,
            defaultAssumeValid: new uint256("0000000001aa875dd078fe2381501d724dade107bea7868fd6586918f4f8421f"), //1499485 // PM-Tech: ChainCoin
            maxMoney: long.MaxValue,
            coinbaseMaturity: 50,
            premineHeight: 2,
            premineReward: Money.Coins(setup.PremineReward),
            proofOfWorkReward: Money.Coins(setup.PoWBlockReward),
            targetTimespan: TimeSpan.FromSeconds(90),
            targetSpacing: setup.TargetSpacing,
            posNoRetargeting: false,
            powNoRetargeting: false,
            powLimit: new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
            minimumChainWork: new uint256("00000000000000000000000000000000000000000000000004b643d48e088b67"), // 1499485 // PM-Tech: ChainCoin,
            isProofOfStake: false,
            lastPowBlock: setup.LastPowBlock,
            proofOfStakeLimit: null,
            proofOfStakeLimitV2: null,
            proofOfStakeReward: Money.Zero,
            proofOfStakeTimestampMask: 0
         //proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
         //proofOfStakeLimitV2: new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
         //proofOfStakeReward: Money.Coins(setup.PoSBlockReward),
         //proofOfStakeTimestampMask: setup.ProofOfStakeTimestampMask
         );
         //{
         //   PosEmptyCoinbase = ChaincoinSetup.Instance.IsPoSv3(),
         //   PosUseTimeFieldInKernalHash = ChaincoinSetup.Instance.IsPoSv3()
         //};

         // TODO: Set your Base58Prefixes
         Base58Prefixes = new byte[12][];
         Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (28) };
         Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (4) };
         Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (28 + 128) };
         Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
         Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
         Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
         Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
         Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
         Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
         Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };

         Bech32Encoders = new Bech32Encoder[2];
         var encoder = new Bech32Encoder(network.CoinTicker.ToLowerInvariant());
         Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
         Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

         Checkpoints = network.Checkpoints;

         DNSSeeds = network.DNS.Select(dns => new DNSSeedData(dns, dns)).ToList();
         SeedNodes = network.Nodes.Select(node => new NBitcoin.Protocol.NetworkAddress(IPAddress.Parse(node), network.DefaultPort)).ToList();

         StandardScriptsRegistry = new ChaincoinStandardScriptsRegistry();

         // 64 below should be changed to TargetSpacingSeconds when we move that field.
         Assert(DefaultBanTimeSeconds <= Consensus.MaxReorgLength * 64 / 2);

         Assert(Genesis.Header.HashMerkleRoot == uint256.Parse(network.HashMerkleRoot));
         
         Assert(Consensus.HashGenesisBlock == uint256.Parse(network.HashGenesisBlock));
         
         RegisterRules(Consensus);
         RegisterMempoolRules(Consensus);
      }

      protected void RegisterRules(IConsensus consensus)
      {
         consensus.ConsensusRules
            .Register<HeaderTimeChecksRule>()
                           //.Register<CheckDifficultyPowRule>()
            .Register<ChaincoinActivationRule>()
            .Register<ChaincoinHeaderVersionRule>();

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

         //consensus.ConsensusRules
         //    .Register<SetActivationDeploymentsFullValidationRule>()

         //    // rules that require the store to be loaded (coinview)
         //    .Register<FetchUtxosetRule>()
         //    .Register<TransactionDuplicationActivationRule>() // implements BIP30
         //    .Register<CheckPowUtxosetPowRule>()// implements BIP68, MaxSigOps and BlockReward calculation
         //    .Register<PushUtxosetRule>()
         //    .Register<FlushUtxosetRule>();
         //consensus.ConsensusRules
         //    .Register<HeaderTimeChecksRule>()
         //    .Register<HeaderTimeChecksPosRule>()
         //    .Register<PosFutureDriftRule>()
         //    .Register<CheckDifficultyPosRule>()
         //    .Register<ChaincoinHeaderVersionRule>()
         //    .Register<ProvenHeaderSizeRule>()
         //    .Register<ProvenHeaderCoinstakeRule>();

         //consensus.ConsensusRules
         //    .Register<BlockMerkleRootRule>()
         //    .Register<PosBlockSignatureRepresentationRule>()
         //    .Register<PosBlockSignatureRule>();

         //consensus.ConsensusRules
         //    .Register<SetActivationDeploymentsPartialValidationRule>()
         //    .Register<PosTimeMaskRule>()

         //    // rules that are inside the method ContextualCheckBlock
         //    .Register<TransactionLocktimeActivationRule>()
         //    .Register<CoinbaseHeightActivationRule>()
         //    .Register<WitnessCommitmentsRule>()
         //    .Register<BlockSizeRule>()

         //    // rules that are inside the method CheckBlock
         //    .Register<EnsureCoinbaseRule>()
         //    .Register<CheckPowTransactionRule>()
         //    .Register<CheckPosTransactionRule>()
         //    .Register<CheckSigOpsRule>()
         //    .Register<PosCoinstakeRule>();

         //consensus.ConsensusRules
         //    .Register<SetActivationDeploymentsFullValidationRule>()

         //    .Register<CheckDifficultyHybridRule>()

         //    // rules that require the store to be loaded (coinview)
         //    .Register<FetchUtxosetRule>()
         //    .Register<TransactionDuplicationActivationRule>()
         //    .Register<CheckPosUtxosetRule>() // implements BIP68, MaxSigOps and BlockReward calculation
         //                                     // Place the PosColdStakingRule after the PosCoinviewRule to ensure that all input scripts have been evaluated
         //                                     // and that the "IsColdCoinStake" flag would have been set by the OP_CHECKCOLDSTAKEVERIFY opcode if applicable.
         //    .Register<PosColdStakingRule>()
         //    .Register<PushUtxosetRule>()
         //    .Register<FlushUtxosetRule>();
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
         var genesisOutputScript = new Script(Op.GetPushOp(Encoders.Hex.DecodeData("04becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8")), OpcodeType.OP_CHECKSIG);
         
         Transaction txNew = consensusFactory.CreateTransaction();
         txNew.Version = 1;
         txNew.AddInput(new TxIn()
         {
            ScriptSig = new Script(Op.GetPushOp(486604799), new Op()
            {
               Code = (OpcodeType)0x1,
               PushData = new[] { (byte)4 }
            }, Op.GetPushOp(Encoders.ASCII.DecodeData(genesisText)))
         });
         txNew.AddOutput(new TxOut()
         {
            Value = genesisReward,
            ScriptPubKey = genesisOutputScript
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
