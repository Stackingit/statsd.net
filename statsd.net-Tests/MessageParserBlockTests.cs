﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using statsd.net.Framework;
using statsd.net.shared.Messages;
using statsd.net.shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using statsd.net;
using statsd.net.shared.Factories;

namespace statsd.net_Tests
{
  [TestClass]
  public class MessageParserBlockTests
  {
    private TransformBlock<string, StatsdMessage> _block;
    private Mock<ISystemMetricsService> _systemMetrics;

    [TestInitialize]
    public void Initialise()
    {
      _systemMetrics = new Mock<ISystemMetricsService>();
      _block = MessageParserBlockFactory.CreateMessageParserBlock(new CancellationToken(), _systemMetrics.Object);
    }

    [TestMethod]
    public void ProcessedALine_IncrementedCounter()
    {
      _systemMetrics.Setup(p => p.LogCount("parser.linesSeen", 1)).Verifiable();

      _block.Post(new Counter("foo", 1).ToString());
      _block.WaitUntilAllItemsProcessed();

      _systemMetrics.VerifyAll();
    }

    [TestMethod]
    public void ProcessedABadLine_IncrementedBadLineCounter()
    {
      _systemMetrics.Setup(p => p.LogCount("parser.badLinesSeen", 1)).Verifiable();

      _block.Post("a bad line");
      _block.WaitUntilAllItemsProcessed();

      _systemMetrics.VerifyAll();
    }
  }
}
