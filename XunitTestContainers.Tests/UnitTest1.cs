using System;
using System.Net.Http;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Xunit;

namespace XunitTestContainers.Tests;

public class UnitTest1
{
  [Fact]
  public async Task ContainerTests()
  {
    var testContainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage("nginx")
      .WithName("nginx")
      .WithPortBinding(80)
      .WithWaitStrategy(Wait.ForUnixContainer()
        .UntilPortIsAvailable(80));

    await using var testContainer = testContainersBuilder.Build();
    await testContainer.StartAsync();

    var dockerClient = new DockerClientConfiguration().CreateClient();
    var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters());
    Assert.Equal(1, containers.Count);

    var client = new HttpClient();
    var response = await client.GetAsync(new Uri("http://localhost"));
    Assert.True(response.IsSuccessStatusCode);

    await testContainer.StopAsync();
  }
}