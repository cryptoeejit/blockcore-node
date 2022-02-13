using System;
using System.IO;
using System.Linq;
using Blockcore.Consensus.ScriptInfo;
using Blockcore.Consensus.TransactionInfo;
using NBitcoin;
using static SpecialTransaction;

namespace Dash.Networks.Consensus
{
   public class DashTransaction : Transaction
   {
      public uint DashVersion => Version & 0xffff;
      public DashTransactionType DashType => (DashTransactionType)((Version >> 16) & 0xffff);
      public byte[] ExtraPayload = new byte[0];
      public ProviderRegistrationTransaction ProRegTx =>
         DashType == DashTransactionType.MasternodeRegistration
            ? new ProviderRegistrationTransaction(ExtraPayload)
            : null;
      public ProviderUpdateServiceTransaction ProUpServTx =>
         DashType == DashTransactionType.UpdateMasternodeService
            ? new ProviderUpdateServiceTransaction(ExtraPayload)
            : null;
      public ProviderUpdateRegistrarTransaction ProUpRegTx =>
         DashType == DashTransactionType.UpdateMasternodeOperator
            ? new ProviderUpdateRegistrarTransaction(ExtraPayload)
            : null;
      public ProviderUpdateRevocationTransaction ProUpRevTx =>
         DashType == DashTransactionType.MasternodeRevocation
            ? new ProviderUpdateRevocationTransaction(ExtraPayload)
            : null;
      public CoinbaseSpecialTransaction CbTx =>
         DashType == DashTransactionType.MasternodeListMerkleProof
            ? new CoinbaseSpecialTransaction(ExtraPayload)
            : null;
      public QuorumCommitmentTransaction QcTx =>
         DashType == DashTransactionType.QuorumCommitment
            ? new QuorumCommitmentTransaction(ExtraPayload)
            : null;

      public override void ReadWrite(BitcoinStream stream)
      {
         base.ReadWrite(stream);
         // Support for Dash 0.13 extraPayload for Special Transactions
         // https://github.com/dashpay/dips/blob/master/dip-0002-special-transactions.md
         if (DashVersion >= 3 && DashType != DashTransactionType.StandardTransaction)
         {
            // Extra payload size is VarInt
            uint extraPayloadSize = (uint)ExtraPayload.Length;
            stream.ReadWriteAsVarInt(ref extraPayloadSize);
            if (ExtraPayload.Length != extraPayloadSize)
               ExtraPayload = new byte[extraPayloadSize];
            stream.ReadWrite(ref ExtraPayload);
         }
      }
   }
}

public abstract class SpecialTransaction
{
   protected SpecialTransaction(byte[] extraPayload)
   {
      data = new BinaryReader(new MemoryStream(extraPayload));
      Version = data.ReadUInt16();
   }

   protected readonly BinaryReader data;

   /// <summary>
   /// Transactions with version >= 3 have a special transaction type in the version code
   /// https://docs.dash.org/en/stable/merchants/technical.html#v0-13-0-integration-notes
   /// 0.14 will add more types: https://github.com/dashpay/dips/blob/master/dip-0002-special-transactions.md
   /// </summary>
   public enum DashTransactionType
   {
      StandardTransaction = 0,
      MasternodeRegistration = 1,
      UpdateMasternodeService = 2,
      UpdateMasternodeOperator = 3,
      MasternodeRevocation = 4,
      MasternodeListMerkleProof = 5,
      QuorumCommitment = 6
   }

   /// <summary>
   /// Version number. Currently set to 1 for all DashTransactionTypes
   /// </summary>
   public ushort Version { get; set; }

   /// <summary>
   /// https://github.com/dashevo/dashcore-lib/blob/master/lib/constants/index.js
   /// </summary>
   public const int PUBKEY_ID_SIZE = 20;
   public const int COMPACT_SIGNATURE_SIZE = 65;
   public const int SHA256_HASH_SIZE = 32;
   public const int BLS_PUBLIC_KEY_SIZE = 48;
   public const int BLS_SIGNATURE_SIZE = 96;
   public const int IpAddressLength = 16;

   protected void MakeSureWeAreAtEndOfPayload()
   {
      if (data.BaseStream.Position < data.BaseStream.Length)
      {
         throw new Exception(
            "Failed to parse payload: raw payload is bigger than expected (pos=" +
            data.BaseStream.Position + ", len=" + data.BaseStream.Length + ")");
      }
   }
}

/// <summary>
/// https://github.com/dashpay/dips/blob/master/dip-0003.md
/// </summary>
public class ProviderRegistrationTransaction : SpecialTransaction
{
   public ProviderRegistrationTransaction(byte[] extraPayload) : base(extraPayload)
   {
      Type = data.ReadUInt16();
      Mode = data.ReadUInt16();
      CollateralHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      CollateralIndex = data.ReadUInt32();
      IpAddress = data.ReadBytes(IpAddressLength);
      Port = BitConverter.ToUInt16(data.ReadBytes(2).Reverse().ToArray(), 0);
      KeyIdOwner = new uint160(data.ReadBytes(PUBKEY_ID_SIZE), true);
      KeyIdOperator = data.ReadBytes(BLS_PUBLIC_KEY_SIZE);
      KeyIdVoting = new uint160(data.ReadBytes(PUBKEY_ID_SIZE), true);
      OperatorReward = data.ReadUInt16();
      var bs = new BitcoinStream(data.BaseStream, false);
      bs.ReadWriteAsVarInt(ref ScriptPayoutSize);
      ScriptPayout = new Script(data.ReadBytes((int)ScriptPayoutSize));
      InputsHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      bs.ReadWriteAsVarInt(ref PayloadSigSize);
      PayloadSig = data.ReadBytes((int)PayloadSigSize);
      MakeSureWeAreAtEndOfPayload();
   }

   public ushort Type { get; set; }
   public ushort Mode { get; set; }
   public uint256 CollateralHash { get; set; }
   public uint CollateralIndex { get; set; }
   public byte[] IpAddress { get; set; }
   public ushort Port { get; set; }
   public uint160 KeyIdOwner { get; set; }
   public byte[] KeyIdOperator { get; set; }
   public uint160 KeyIdVoting { get; set; }
   public ushort OperatorReward { get; set; }
   public uint ScriptPayoutSize;
   public Script ScriptPayout { get; set; }
   public uint256 InputsHash { get; set; }
   public uint PayloadSigSize;
   public byte[] PayloadSig { get; set; }
}

public class ProviderUpdateServiceTransaction : SpecialTransaction
{
   public ProviderUpdateServiceTransaction(byte[] extraPayload) : base(extraPayload)
   {
      ProTXHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      IpAddress = data.ReadBytes(IpAddressLength);
      Port = BitConverter.ToUInt16(data.ReadBytes(2).Reverse().ToArray(), 0);
      var bs = new BitcoinStream(data.BaseStream, false);
      bs.ReadWriteAsVarInt(ref ScriptOperatorPayoutSize);
      ScriptOperatorPayout = new Script(data.ReadBytes((int)ScriptOperatorPayoutSize));
      InputsHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      PayloadSig = data.ReadBytes(BLS_SIGNATURE_SIZE);
      MakeSureWeAreAtEndOfPayload();
   }

   public uint256 ProTXHash { get; set; }
   public byte[] IpAddress { get; set; }
   public ushort Port { get; set; }
   public uint ScriptOperatorPayoutSize;
   public Script ScriptOperatorPayout { get; set; }
   public uint256 InputsHash { get; set; }
   public byte[] PayloadSig { get; set; }
}

public class ProviderUpdateRegistrarTransaction : SpecialTransaction
{
   public ProviderUpdateRegistrarTransaction(byte[] extraPayload) : base(extraPayload)
   {
      ProTXHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      Mode = data.ReadUInt16();
      PubKeyOperator = data.ReadBytes(BLS_PUBLIC_KEY_SIZE);
      KeyIdVoting = new uint160(data.ReadBytes(PUBKEY_ID_SIZE), true);
      var bs = new BitcoinStream(data.BaseStream, false);
      bs.ReadWriteAsVarInt(ref ScriptPayoutSize);
      ScriptPayout = new Script(data.ReadBytes((int)ScriptPayoutSize));
      InputsHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      if (data.BaseStream.Position < data.BaseStream.Length)
      {
         bs.ReadWriteAsVarInt(ref PayloadSigSize);
         PayloadSig = data.ReadBytes((int)PayloadSigSize);
      }
      else
      {
         PayloadSig = new byte[0];
      }
      MakeSureWeAreAtEndOfPayload();
   }

   public uint256 ProTXHash { get; set; }
   public ushort Mode { get; set; }
   public byte[] PubKeyOperator { get; set; }
   public uint160 KeyIdVoting { get; set; }
   public uint ScriptPayoutSize;
   public Script ScriptPayout { get; set; }
   public uint256 InputsHash { get; set; }
   public uint PayloadSigSize;
   public byte[] PayloadSig { get; set; }
}

public class ProviderUpdateRevocationTransaction : SpecialTransaction
{
   public ProviderUpdateRevocationTransaction(byte[] extraPayload) : base(extraPayload)
   {
      ProTXHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      Reason = data.ReadUInt16();
      InputsHash = new uint256(data.ReadBytes(SHA256_HASH_SIZE), true);
      PayloadSig = data.ReadBytes(BLS_SIGNATURE_SIZE);
      MakeSureWeAreAtEndOfPayload();
   }

   public uint256 ProTXHash { get; set; }
   public ushort Reason { get; set; }
   public uint256 InputsHash { get; set; }
   public uint PayloadSigSize;
   public byte[] PayloadSig { get; set; }
}

public abstract class SpecialTransactionWithHeight : SpecialTransaction
{
   protected SpecialTransactionWithHeight(byte[] extraPayload) : base(extraPayload)
   {
      Height = data.ReadUInt32();
   }

   /// <summary>
   /// Height of the block
   /// </summary>
   public uint Height { get; set; }
}

/// <summary>
/// For DashTransactionType.MasternodeListMerkleProof
/// https://github.com/dashpay/dips/blob/master/dip-0004.md
/// Only needs deserialization here, ExtraPayload can still be serialized
/// </summary>
public class CoinbaseSpecialTransaction : SpecialTransactionWithHeight
{
   public CoinbaseSpecialTransaction(byte[] extraPayload) : base(extraPayload)
   {
      MerkleRootMNList = new uint256(data.ReadBytes(SHA256_HASH_SIZE));
      MakeSureWeAreAtEndOfPayload();
   }

   /// <summary>
   /// Merkle root of the masternode list
   /// </summary>
   public uint256 MerkleRootMNList { get; set; }
}

/// <summary>
/// https://github.com/dashevo/dashcore-lib/blob/master/lib/transaction/payload/commitmenttxpayload.js
/// </summary>
public class QuorumCommitmentTransaction : SpecialTransactionWithHeight
{
   public QuorumCommitmentTransaction(byte[] extraPayload) : base(extraPayload)
   {
      Commitment = new QuorumCommitment(data);
      MakeSureWeAreAtEndOfPayload();
   }

   public QuorumCommitment Commitment { get; set; }
}

public class QuorumCommitment
{
   public QuorumCommitment(BinaryReader data)
   {
      QfcVersion = data.ReadUInt16();
      LlmqType = data.ReadByte();
      QuorumHash = new uint256(data.ReadBytes(SpecialTransaction.SHA256_HASH_SIZE));
      var bs = new BitcoinStream(data.BaseStream, false);
      bs.ReadWriteAsVarInt(ref SignersSize);
      Signers = data.ReadBytes(((int)SignersSize + 7) / 8);
      bs.ReadWriteAsVarInt(ref ValidMembersSize);
      ValidMembers = data.ReadBytes(((int)ValidMembersSize + 7) / 8);
      QuorumPublicKey = data.ReadBytes(SpecialTransaction.BLS_PUBLIC_KEY_SIZE);
      QuorumVvecHash = new uint256(data.ReadBytes(SpecialTransaction.SHA256_HASH_SIZE));
      QuorumSig = data.ReadBytes(SpecialTransaction.BLS_SIGNATURE_SIZE);
      Sig = data.ReadBytes(SpecialTransaction.BLS_SIGNATURE_SIZE);
   }

   public ushort QfcVersion { get; set; }
   public byte LlmqType { get; set; }
   public uint256 QuorumHash { get; set; }
   public uint SignersSize;
   public byte[] Signers { get; set; }
   public uint ValidMembersSize;
   public byte[] ValidMembers { get; set; }
   public byte[] QuorumPublicKey { get; set; }
   public uint256 QuorumVvecHash { get; set; }
   public byte[] QuorumSig { get; set; }
   public byte[] Sig { get; set; }
}

// NBitcoin
//   /// <summary>
//   /// https://docs.dash.org/en/stable/merchants/technical.html#v0-13-0-integration-notes
//   /// </summary>
//   public class DashTransaction : Transaction
//   {
//      public uint DashVersion => Version & 0xffff;
//      public DashTransactionType DashType => (DashTransactionType)((Version >> 16) & 0xffff);
//      public byte[] ExtraPayload = new byte[0];
//      public ProviderRegistrationTransaction ProRegTx =>
//         DashType == DashTransactionType.MasternodeRegistration
//            ? new ProviderRegistrationTransaction(ExtraPayload)
//            : null;
//      public ProviderUpdateServiceTransaction ProUpServTx =>
//         DashType == DashTransactionType.UpdateMasternodeService
//            ? new ProviderUpdateServiceTransaction(ExtraPayload)
//            : null;
//      public ProviderUpdateRegistrarTransaction ProUpRegTx =>
//         DashType == DashTransactionType.UpdateMasternodeOperator
//            ? new ProviderUpdateRegistrarTransaction(ExtraPayload)
//            : null;
//      public ProviderUpdateRevocationTransaction ProUpRevTx =>
//         DashType == DashTransactionType.MasternodeRevocation
//            ? new ProviderUpdateRevocationTransaction(ExtraPayload)
//            : null;
//      public CoinbaseSpecialTransaction CbTx =>
//         DashType == DashTransactionType.MasternodeListMerkleProof
//            ? new CoinbaseSpecialTransaction(ExtraPayload)
//            : null;
//      public QuorumCommitmentTransaction QcTx =>
//         DashType == DashTransactionType.QuorumCommitment
//            ? new QuorumCommitmentTransaction(ExtraPayload)
//            : null;

//      public override void ReadWrite(BitcoinStream stream)
//      {
//         base.ReadWrite(stream);
//         // Support for Dash 0.13 extraPayload for Special Transactions
//         // https://github.com/dashpay/dips/blob/master/dip-0002-special-transactions.md
//         if (DashVersion >= 3 && DashType != DashTransactionType.StandardTransaction)
//         {
//            // Extra payload size is VarInt
//            uint extraPayloadSize = (uint)ExtraPayload.Length;
//            stream.ReadWriteAsVarInt(ref extraPayloadSize);
//            if (ExtraPayload.Length != extraPayloadSize)
//               ExtraPayload = new byte[extraPayloadSize];
//            stream.ReadWrite(ref ExtraPayload);
//         }
//      }
//   }
