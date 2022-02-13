using System;
using System.Collections.Generic;
using Blockcore.Base.Deployments;
using Blockcore.Consensus;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;

namespace Dash.Networks.Consensus
{
   public class DashConsensus : IConsensus
   {
      /// <inheritdoc />
      public long CoinbaseMaturity { get; set; }

      /// <inheritdoc />
      public Money PremineReward { get; }

      /// <inheritdoc />
      public long PremineHeight { get; }

      /// <inheritdoc />
      public Money ProofOfWorkReward { get; }

      /// <inheritdoc />
      public Money ProofOfStakeReward { get; }

      /// <inheritdoc />
      public uint MaxReorgLength { get; private set; }

      /// <inheritdoc />
      public long MaxMoney { get; }

      public ConsensusOptions Options { get; set; }

      public BuriedDeploymentsArray BuriedDeployments { get; }

      public IBIP9DeploymentsArray BIP9Deployments { get; }

      public int SubsidyHalvingInterval { get; }

      public int MajorityEnforceBlockUpgrade { get; }

      public int MajorityRejectBlockOutdated { get; }

      public int MajorityWindow { get; }

      public uint256 BIP34Hash { get; }

      public Target PowLimit { get; }
      public Target PowLimit2 { get; }
      public int PowLimit2Height { get; }
      public uint PowLimit2Time { get; }

      public TimeSpan TargetTimespan { get; }

      public TimeSpan TargetSpacing { get; }

      public bool PowAllowMinDifficultyBlocks { get; }

      /// <inheritdoc />
      public bool PosNoRetargeting { get; }

      /// <inheritdoc />
      public bool PowNoRetargeting { get; }

      public uint256 HashGenesisBlock { get; }

      /// <inheritdoc />
      public uint256 MinimumChainWork { get; }

      public int MinerConfirmationWindow { get; set; }

      /// <inheritdoc />
      public int CoinType { get; }

      public BigInteger ProofOfStakeLimit { get; }

      public BigInteger ProofOfStakeLimitV2 { get; }

      /// <inheritdoc />
      public int LastPOWBlock { get; set; }

      /// <inheritdoc />
      public bool IsProofOfStake { get; }

      /// <inheritdoc />
      public bool PosEmptyCoinbase { get; set; }

      /// <inheritdoc />
      public bool PosUseTimeFieldInKernalHash { get; set; }

      /// <inheritdoc />
      public uint ProofOfStakeTimestampMask { get; set; }

      /// <inheritdoc />
      public uint256 DefaultAssumeValid { get; }

      /// <inheritdoc />
      public ConsensusFactory ConsensusFactory { get; }

      /// <inheritdoc />
      public ConsensusRules ConsensusRules { get; }

      /// <inheritdoc />
      public List<Type> MempoolRules { get; set; }

      public DashConsensus(
          ConsensusFactory consensusFactory,
          ConsensusOptions consensusOptions,
          int coinType,
          uint256 hashGenesisBlock,
          int subsidyHalvingInterval,
          int majorityEnforceBlockUpgrade,
          int majorityRejectBlockOutdated,
          int majorityWindow,
          BuriedDeploymentsArray buriedDeployments,
          IBIP9DeploymentsArray bip9Deployments,
          uint256 bip34Hash,
          int minerConfirmationWindow,
          uint maxReorgLength,
          uint256 defaultAssumeValid,
          long maxMoney,
          long coinbaseMaturity,
          long premineHeight,
          Money premineReward,
          Money proofOfWorkReward,
          TimeSpan targetTimespan,
          TimeSpan targetSpacing,
          bool powAllowMinDifficultyBlocks,
          bool posNoRetargeting,
          bool powNoRetargeting,
          Target powLimit,
          uint256 minimumChainWork,
          bool isProofOfStake,
          int lastPowBlock,
          BigInteger proofOfStakeLimit,
          BigInteger proofOfStakeLimitV2,
          Money proofOfStakeReward,
          uint proofOfStakeTimestampMask)
      {
         CoinbaseMaturity = coinbaseMaturity;
         PremineReward = premineReward;
         PremineHeight = premineHeight;
         ProofOfWorkReward = proofOfWorkReward;
         ProofOfStakeReward = proofOfStakeReward;
         MaxReorgLength = maxReorgLength;
         MaxMoney = maxMoney;
         Options = consensusOptions;
         BuriedDeployments = buriedDeployments;
         BIP9Deployments = bip9Deployments;
         SubsidyHalvingInterval = subsidyHalvingInterval;
         MajorityEnforceBlockUpgrade = majorityEnforceBlockUpgrade;
         MajorityRejectBlockOutdated = majorityRejectBlockOutdated;
         MajorityWindow = majorityWindow;
         BIP34Hash = bip34Hash;
         PowLimit = powLimit;
         TargetTimespan = targetTimespan;
         TargetSpacing = targetSpacing;
         PowAllowMinDifficultyBlocks = powAllowMinDifficultyBlocks;
         PosNoRetargeting = posNoRetargeting;
         PowNoRetargeting = powNoRetargeting;
         HashGenesisBlock = hashGenesisBlock;
         MinimumChainWork = minimumChainWork;
         MinerConfirmationWindow = minerConfirmationWindow;
         CoinType = coinType;
         ProofOfStakeLimit = proofOfStakeLimit;
         ProofOfStakeLimitV2 = proofOfStakeLimitV2;
         LastPOWBlock = lastPowBlock;
         IsProofOfStake = isProofOfStake;
         DefaultAssumeValid = defaultAssumeValid;
         ConsensusFactory = consensusFactory;
         ConsensusRules = new ConsensusRules();
         MempoolRules = new List<Type>();
         ProofOfStakeTimestampMask = proofOfStakeTimestampMask;
      }
   }
}
