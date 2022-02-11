using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Consensus;
using Blockcore.Consensus.BlockInfo;

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
   }
}
