using NUnit.Framework;
using Scalar.Common;
using Scalar.Tests.Should;
using Scalar.UnitTests.Mock.Upgrader;
using Scalar.Upgrader;
using System.Collections.Generic;

namespace Scalar.UnitTests.Upgrader
{
    [TestFixture]
    public class UpgradeOrchestratorWithGitHubUpgraderTests : UpgradeTests
    {
        private UpgradeOrchestrator orchestrator;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.orchestrator = new WindowsUpgradeOrchestrator(
                this.Upgrader,
                this.Tracer,
                this.FileSystem,
                this.PrerunChecker,
                input: null,
                output: this.Output);
            this.PrerunChecker.SetCommandToRerun("`scalar upgrade --confirm`");
        }

        [TestCase]
        public void UpgradeNoError()
        {
            this.RunUpgrade().ShouldEqual(ReturnCode.Success);
            this.Tracer.RelatedErrorEvents.ShouldBeEmpty();
        }


        [TestCase]
        public void AbortOnBlockingProcess()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.PrerunChecker.SetReturnTrueOnCheck(MockInstallerPrerunChecker.FailOnCheckType.BlockingProcessesRunning);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "ERROR: Blocking processes are running.",
                    $"Run `scalar upgrade --confirm` again after quitting these processes - git"
                },
                expectedErrors: null,
                expectedWarnings: new List<string>
                {
                    $"Run `scalar upgrade --confirm` again after quitting these processes - git"
                });
        }

        [TestCase]
        public void DownloadDirectoryCreationError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.CreateDownloadDirectory);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "Error creating download directory"
                },
                expectedErrors: new List<string>
                {
                    "Error creating download directory"
                });
        }

        [TestCase]
        public void ScalarDownloadError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.ScalarDownload);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "Error downloading Scalar from GitHub"
                },
                expectedErrors: new List<string>
                {
                    "Error downloading Scalar from GitHub"
                });
        }

        [TestCase]
        public void GitDownloadError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.GitDownload);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "Error downloading Git from GitHub"
                },
                expectedErrors: new List<string>
                {
                    "Error downloading Git from GitHub"
                });
        }

        [TestCase]
        public void GitInstallationArgs()
        {
            this.RunUpgrade().ShouldEqual(ReturnCode.Success);

            Dictionary<string, string> gitInstallerInfo;
            this.Upgrader.InstallerArgs.ShouldBeNonEmpty();
            this.Upgrader.InstallerArgs.TryGetValue("Git", out gitInstallerInfo).ShouldBeTrue();

            string args;
            gitInstallerInfo.TryGetValue("Args", out args).ShouldBeTrue();
            args.ShouldContain(new string[] { "/VERYSILENT", "/CLOSEAPPLICATIONS", "/SUPPRESSMSGBOXES", "/NORESTART", "/Log" });
        }

        [TestCase]
        public void GitInstallError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.GitInstall);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "Git installation failed"
                },
                expectedErrors: new List<string>
                {
                    "Git installation failed"
                });
        }

        [TestCase]
        public void GitInstallerAuthenticodeError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.GitAuthenticodeCheck);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "hash of the file does not match the hash stored in the digital signature"
                },
                expectedErrors: new List<string>
                {
                    "hash of the file does not match the hash stored in the digital signature"
                });
        }

        [TestCase]
        public void ScalarInstallationArgs()
        {
            this.RunUpgrade().ShouldEqual(ReturnCode.Success);

            Dictionary<string, string> gitInstallerInfo;
            this.Upgrader.InstallerArgs.ShouldBeNonEmpty();
            this.Upgrader.InstallerArgs.TryGetValue("Scalar", out gitInstallerInfo).ShouldBeTrue();

            string args;
            gitInstallerInfo.TryGetValue("Args", out args).ShouldBeTrue();
            args.ShouldContain(new string[] { "/VERYSILENT", "/CLOSEAPPLICATIONS", "/SUPPRESSMSGBOXES", "/NORESTART", "/Log" });
        }

        [TestCase]
        public void ScalarInstallError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.ScalarInstall);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "Scalar installation failed"
                },
                expectedErrors: new List<string>
                {
                    "Scalar installation failed"
                });
        }

        [TestCase]
        public void ScalarInstallerAuthenticodeError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.ScalarAuthenticodeCheck);
                },
                expectedReturn: ReturnCode.GenericError,
                expectedOutput: new List<string>
                {
                    "hash of the file does not match the hash stored in the digital signature"
                },
                expectedErrors: new List<string>
                {
                    "hash of the file does not match the hash stored in the digital signature"
                });
        }

        [TestCase]
        public void ScalarCleanupError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.ScalarCleanup);
                },
                expectedReturn: ReturnCode.Success,
                expectedOutput: new List<string>
                {
                },
                expectedErrors: new List<string>
                {
                    "Error deleting downloaded Scalar installer."
                });
        }

        [TestCase]
        public void GitCleanupError()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetFailOnAction(MockGitHubUpgrader.ActionType.GitCleanup);
                },
                expectedReturn: ReturnCode.Success,
                expectedOutput: new List<string>
                {
                },
                expectedErrors: new List<string>
                {
                    "Error deleting downloaded Git installer."
                });
        }

        [TestCase]
        public void DryRunDoesNotRunInstallerExes()
        {
            this.ConfigureRunAndVerify(
                configure: () =>
                {
                    this.Upgrader.SetDryRun(true);
                    this.Upgrader.InstallerExeLaunched = false;
                    this.SetUpgradeRing("Slow");
                    this.Upgrader.PretendNewReleaseAvailableAtRemote(
                        upgradeVersion: NewerThanLocalVersion,
                        remoteRing: GitHubUpgrader.GitHubUpgraderConfig.RingType.Slow);
                },
                expectedReturn: ReturnCode.Success,
                expectedOutput: new List<string>
                {
                    "Installing Git",
                    "Installing Scalar",
                    "Upgrade completed successfully."
                },
                expectedErrors: null);

            this.Upgrader.InstallerExeLaunched.ShouldBeFalse();
        }

        protected override ReturnCode RunUpgrade()
        {
            this.orchestrator.Execute();
            return this.orchestrator.ExitCode;
        }
    }
}
