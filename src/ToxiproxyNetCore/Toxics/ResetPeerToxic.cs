namespace Toxiproxy.Net.Toxics

{

    public sealed class ResetPeerToxic : ToxicBase

    {

        /// <summary>

        /// The attributes for the ResetPeer Toxic

        /// </summary>

        public class ToxicAttributes

        {

            public int Timeout { get; set; }

        }



        /// <summary>

        /// Initializes a new instance of the <see cref="ResetPeerToxic"/> class.

        /// </summary>

        public ResetPeerToxic()

        {

            Attributes = new ToxicAttributes();

        }



        public ToxicAttributes Attributes { get; set; }



        public override string Type => ToxicTypenames.ResetPeerToxic;

    }

}