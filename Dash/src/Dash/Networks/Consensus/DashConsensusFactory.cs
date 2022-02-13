using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Consensus;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus.TransactionInfo;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Dash.Networks.Consensus
{
   public class DashConsensusFactory : ConsensusFactory
   {
      public DashConsensusFactory() : base()
      {
      }

      public override BlockHeader CreateBlockHeader()
      {
         return new DashBlockHeader();
      }

      public override Transaction CreateTransaction()
      {
         return new DashTransaction();
      }

      public override Transaction CreateTransaction(byte[] bytes)
      {
         if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

         var transaction = new DashTransaction();
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
