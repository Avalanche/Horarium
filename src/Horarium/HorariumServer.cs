﻿using System;
using System.Runtime.CompilerServices;
using Horarium.Handlers;
using Horarium.Interfaces;
using Horarium.Repository;

[assembly: InternalsVisibleTo("Horarium.Test")]

namespace Horarium
{
    public class HorariumServer : HorariumClient, IHorarium
    {
        private readonly HorariumSettings _settings;
        private IRunnerJobs _runnerJobs;
        private readonly IAdderJobs _adderJobs;

        private readonly IJobRepository _jobRepository;

        private readonly TimeSpan _timeoutStop = TimeSpan.FromSeconds(5);

        public HorariumServer(IJobRepository jobRepository)
            : this(jobRepository, new HorariumSettings())
        {
        }

        public HorariumServer(IJobRepository jobRepository, HorariumSettings settings)
            : base(jobRepository, settings)
        {
            _settings = settings;
            _adderJobs = new AdderJobs(jobRepository, _settings.JsonSerializerSettings);
            _jobRepository = jobRepository;
        }

        public void Start()
        {
            var executorJob = new ExecutorJob(_jobRepository, _adderJobs, _settings);

            _runnerJobs = new RunnerJobs(_jobRepository, _settings, _settings.JsonSerializerSettings, _settings.Logger,
                executorJob);
            _runnerJobs.Start();
        }

        public void Stop()
        {
            _runnerJobs.Stop().Wait(_timeoutStop);
        }

        public new void Dispose()
        {
            Stop();
        }
    }
}