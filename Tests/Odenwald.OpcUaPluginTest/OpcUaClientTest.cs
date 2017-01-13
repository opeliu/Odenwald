﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


using Opc.Ua.Client;
using Opc.Ua;
using Odenwald.OpcUaPlugin;

namespace Odenwald.OpcUaPluginTest
{
    public class OpcUaClientTest
    {
        [Fact]
        public void OpcUaHelperTest()
        {
            //config
            ApplicationConfiguration l_applicationConfig = new ApplicationConfiguration()
            {
               
                ApplicationName = "OdenwaldUnitTest",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = @"Windows",
                        StorePath = @"CurrentUser\My",
                        SubjectName = Utils.Format(@"CN={0}, DC={1}",
                       "Odenwald",
                       System.Net.Dns.GetHostName())
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = @"Windows",
                        StorePath = @"CurrentUser\TrustedPeople",
                    },
                    NonceLength = 32,
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };
            l_applicationConfig.Validate(ApplicationType.Client);
            if (l_applicationConfig.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                l_applicationConfig.CertificateValidator.CertificateValidation += (s, e) =>
                { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
            }

            Session l_session = Session.Create(l_applicationConfig, new ConfiguredEndpoint(null, new EndpointDescription("opc.tcp://ac-l32-m7-lth1:51210/UA/SampleServer")), true, "OdenwaldUnitTest", 60000, null, null);//EndpointDescription need to be changed according to your OPC server

            string[] browsePaths = new string[]
           {
                "Data",
                "Dynamic",
                "Scalar",
                "Int32Value"
           };
            string nodeRelativePath = "/Data/Dynamic/Scalar/Int32Value/";
            

            string[] relativePaths = { "/Data/Dynamic/Scalar/Int32Value/" }; // nodeRelativePath.Split('.');
            
            var nodeList = OpcUaHelper.TranslateBrowsePaths(l_session, ObjectIds.ObjectsFolder, l_session.NamespaceUris, browsePaths);

            Assert.NotNull(nodeList);
            Assert.Equal(1, nodeList.Count);

        }
    }
}
