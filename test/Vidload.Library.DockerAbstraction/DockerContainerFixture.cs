using System;
using Docker.DotNet;

namespace Vidload.Library.DockerAbstraction {
  public class PortMapping {
    public readonly int SourcePort;
    public readonly int TargetPort;

    public PortMapping(int sourcePort, int targetPort) {
      this.SourcePort = sourcePort;
      this.TargetPort = targetPort;
    }
  }

  public class VolumeMapping {
    public readonly string HostVolumePath;
    public readonly string ContainerVolumePath;
    
    public VolumeMapping(string hostVolumePath, string containerVolumePath) {
      HostVolumePath = hostVolumePath;
      ContainerVolumePath = containerVolumePath;
    }
  }

  public class ContainerInformation {
    public readonly string ImageName;
    public readonly string ImageTag;
    public readonly PortMapping PortMapping;
    public readonly VolumeMapping VolumeMapping;
    public readonly Func<bool> VerifyOperationalStatus;

    public ContainerInformation(string imageName, string imageTag) {
      ImageName = imageName;
      ImageTag = imageTag;
    }
    
    public ContainerInformation(string imageName, string imageTag, PortMapping portMapping) {
      ImageName = imageName;
      ImageTag = imageTag;
      PortMapping = portMapping;
    }
    
    public ContainerInformation(string imageName, string imageTag, PortMapping portMapping, VolumeMapping volumeMapping, Func<bool> verifyOperationalStatus) {
      ImageName = imageName;
      ImageTag = imageTag;
      PortMapping = portMapping;
      VolumeMapping = volumeMapping;
      VerifyOperationalStatus = verifyOperationalStatus;
    }
  }

  public class DockerConfiguration {
    public readonly string DockerDeamonSocket;
  }
  
  public class DockerContainerFixture : IDisposable {

    private DockerClient _dockerClient;
    
    public DockerContainerFixture() {

    }

    public void Configure(DockerConfiguration dockerConfiguration, ContainerInformation containerInformation) {
      _dockerClient = new DockerClientConfiguration(new Uri(dockerConfiguration.DockerDeamonSocket)).CreateClient();
    }

    public void StartContainer() {
      
    }
    
    
    
    public void Dispose()
    {
    }
  }
}
