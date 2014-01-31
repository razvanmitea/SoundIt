using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech;
using Android.Media;
using Android.Util;
using System.IO;

namespace SoundIt
{
	[Activity (Label = "Welcome to SoundIt")]
	public class MainActivity : Activity
	{
		int count = 1;
		Int16[] audioBuffer;
		AudioRecord ar;
		AudioTrack audioTrack;
		int bufferSize = 0;


		protected override void OnCreate (Bundle bundle)
		{

			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button buttonRec = FindViewById<Button> (Resource.Id.myButton);
			Button buttonPlay = FindViewById<Button> (Resource.Id.btnPlay);
            ToggleButton buttonRecT = FindViewById<ToggleButton> (Resource.Id.toggleButton1);
            EditText editTextBox = FindViewById<EditText>(Resource.Id.editText1);

			ar = findAudioRecord ();
			audioBuffer = new Int16[bufferSize];
			//ar.Release ();

            buttonRecT.Click += (sender, e) => {

                //Worker/recording thread. Should message the UI/main thread whenever buffer gets full.
                Thread t = new Thread(new ThreadStart(() => {
                    int i = 0;
                    //recording loop
                    while (buttonRecT.Checked) {
                        Thread.Sleep(3000); //used temporarily for demoing threaded recording/not to be used in the actual implementation 
                        //needed as we can stop recording while thread is sleeping
                        if(buttonRecT.Checked)
                        {
                            //update UI/main thread
                            RunOnUiThread(() => editTextBox.Text += (3*i++) + "s ");
                        }
                    }
                }));

                if (buttonRecT.Checked) {
                    editTextBox.Text += "Recording.. ";
                    t.Start();
                }
                else
                {
                    editTextBox.Text += "Stopped.. ";
                }
                
            };

			buttonRec.Click += delegate {

				ar.StartRecording();
				while (true) {
					try
					{
						// Keep reading the buffer 
						//while there is audio input.
						ar.Read(audioBuffer, 0,	audioBuffer.Length);

						if(count++ > audioBuffer.Length)
						{
							ar.Stop();
							break;
						}
						// Write out the audio file.
					}
					catch (Exception ex)
					{
						Console.Out.WriteLine(ex.Message);
						break;
					}
				}
			};

			buttonPlay.Click += (sender, e) => 
			{
				int minimumBufferSize = AudioTrack.GetMinBufferSize(ar.SampleRate, ChannelOut.Mono, Android.Media.Encoding.Pcm16bit);
                Log.Debug("SoundIt", "minimumBufferSize = " + minimumBufferSize);
                Log.Debug("SoundIt", "audioBuffer.Length = " + audioBuffer.Length);

				 audioTrack = new AudioTrack(
					// Stream type
					Android.Media.Stream.Music,
					// Frequency
					ar.SampleRate,
					// Mono or stereo
					ChannelConfiguration.Mono,
					// Audio encoding
					Android.Media.Encoding.Pcm16bit,
					// Length of the audio clip.
					(minimumBufferSize < audioBuffer.Length ? audioBuffer.Length : minimumBufferSize),
					// Mode. Stream or static.
					AudioTrackMode.Static);
				
				audioTrack.Play();
				audioTrack.Write(audioBuffer, 0, audioBuffer.Length);
			};
           
		}

		public AudioRecord findAudioRecord() 
		{
			
		 //int[] mSampleRates = new int[] { 8000, 11025, 22050, 44100 };
         int[] mSampleRates = new int[] { 44100, 22050 };

			foreach (int rate in mSampleRates) {
				foreach (var channelConfig in new ChannelIn[] { ChannelIn.Mono, ChannelIn.Stereo }) {
					try {
						Log.Debug ("SoundIt", "Attempting rate " + rate + "Hz, bits: " + Android.Media.Encoding.Pcm16bit + ", channel: " + channelConfig);
						bufferSize = AudioRecord.GetMinBufferSize (rate, channelConfig, Android.Media.Encoding.Pcm16bit);
                        Log.Debug("SoundIt", "Buffer size was set to bufferSize = " + bufferSize);

						if (bufferSize > 0) {
							// check if we can instantiate and have a success
							AudioRecord recorder = new AudioRecord (AudioSource.Mic, rate, channelConfig, Android.Media.Encoding.Pcm16bit, bufferSize);

							if (recorder.State == State.Initialized)
								return recorder;
						}
					} catch (Exception e) {
						Log.Error ("Error", rate + "Exception, keep trying.", e);
					}
				}
			}
			return null;
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //Apparently preloading a menu from a xml doesn't work anymore

            //Log.Debug("SoundIt", "Menu created!");
            //MenuInflater.Inflate(Resource.Menu.optionsMenu, menu); 

            menu.Add(0, 1, 1, "Quit");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 1: //Quit button
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

    }



}


