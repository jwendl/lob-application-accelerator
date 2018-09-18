using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.App.Model
{
    public class TeamsConfiguration: TableEntity
    {
        //PartitionKey: "TeamsTask"
        //Rowkey: Guid
        public string Name { get; set; }
        public string Description { get; set; }
    }


    public class MemeberConfiguration : TableEntity
    {
        //PartitionKey: TeamsConfiguration.RowKey
        //Rowkey: Guid
        public string Name { get; set; }
    }

    public class ChannelConfiguration : TableEntity
    {
        //PartitionKey: TeamsConfiguration.RowKey
        //Rowkey: Guid
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ChannelMembersConfiguration : TableEntity
    {
        //PartitionKey: TeamsConfiguration.RowKey
        //Rowkey: ChannelConfiguration.RowKey
        public string MemberName { get; set; }
    }
}
