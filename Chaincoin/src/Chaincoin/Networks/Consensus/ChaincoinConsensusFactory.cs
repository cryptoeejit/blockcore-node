using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Consensus;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus.TransactionInfo;
using NBitcoin.DataEncoders;
using NBitcoin;

namespace Chaincoin.Networks.Consensus
{
    public class ChaincoinConsensusFactory : ConsensusFactory
    {
      public ChaincoinConsensusFactory() : base()
      {
      }
            
      public override BlockHeader CreateBlockHeader()
      {
         return new ChaincoinBlockHeader();
      }

      public override Transaction CreateTransaction()
      {
         return new ChaincoinTransaction();
      }

      public override Transaction CreateTransaction(byte[] bytes)
      {
         if (bytes == null)
               throw new ArgumentNullException(nameof(bytes));

         var transaction = new ChaincoinTransaction();
         transaction.ReadWrite(bytes, this);
         return transaction;
      }

      public override Transaction CreateTransaction(string hex)
      {
         if (hex == null)
               throw new ArgumentNullException(nameof(hex));

         return CreateTransaction(Encoders.Hex.DecodeData(hex));
      }

   }
}
