using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Consensus;
using Blockcore.Consensus.BlockInfo;

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
   }
}
// NBitcoin
//   public class DashConsensusFactory : ConsensusFactory
//   {
//      private DashConsensusFactory()
//      {
//      }

//      // ReSharper disable once MemberHidesStaticFromOuterClass
//      public static DashConsensusFactory Instance { get; } = new DashConsensusFactory();

//      public override BlockHeader CreateBlockHeader()
//      {
//         return new DashBlockHeader();
//      }

//      public override Block CreateBlock()
//      {
//         return new DashBlock(new DashBlockHeader());
//      }

//      public override Transaction CreateTransaction()
//      {
//         return new DashTransaction();
//      }
//   }
