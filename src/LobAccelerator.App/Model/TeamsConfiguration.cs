using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace LobAccelerator.App.Model
{
    /// <summary>
    /// Team configuration intended to be used on Azure Storage
    /// </summary>
    public class TeamsConfiguration : TableEntity
    {
        //PartitionKey: "TeamsTask"
        //Rowkey: Guid
        public string Name { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Member configuration intended to be used on Azure Storage
    /// </summary>
    public class MemeberConfiguration : TableEntity
    {
        //PartitionKey: TeamsConfiguration.RowKey
        //Rowkey: Guid
        public string Name { get; set; }
    }

    /// <summary>
    /// Channel configuration intended to be used on Azure Storage
    /// </summary>
    public class ChannelConfiguration : TableEntity
    {
        //PartitionKey: TeamsConfiguration.RowKey
        //Rowkey: Guid
        public string Name { get; set; }
        public string Description { get; set; }
        public string MemberList { get; set; }

        public IEnumerable<string> GetMembersList()
        {
            return MemberList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
