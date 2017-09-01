using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Globalization;

namespace GPXWriter
{
    namespace Main
    {
        public class GPX
        {
            private string _Filename;
            private ArrayList _Tracks;
            private ArrayList _WayPoints;

            public string Filename
            {
                get
                {
                    return _Filename;
                }
                set
                {
                    _Filename = Path.GetFullPath(value);
                }
            }
            public ArrayList Tracks
            {
                get
                {
                    return ArrayList.ReadOnly(_Tracks);
                }                    
            }
            public ArrayList WayPoints
            {
                get
                {
                    return ArrayList.ReadOnly(_WayPoints);
                } 
            }

            public GPX(string Filename)
            {
                _Filename = Path.GetFullPath(Filename);
                _Tracks = new ArrayList();
                _WayPoints = new ArrayList();
            }

            public void WriteGPX()
            {
                GPXWrite TmpWriteCache = new GPXWrite(this);
                TmpWriteCache.Write();
            }
            public void AddTrack(Utils.Track track)
            {
                _Tracks.Add(track);
            }
            public void AddPoint(GPXWriter.Utils.Point point)
            {
                _WayPoints.Add(point);
            }

        }
        public class GPXWrite
        {
            private GPX _GPX;
            private XmlTextWriter _OutputFile;
            
            public GPXWrite(GPX gpx)
            {
                if (gpx == null)
                {
                    throw new Exception("GPX gpx musn't be empty");
                }
                   
                _GPX = gpx;                    
                    
            }

            private void WriteHeader()
            {
                _OutputFile.WriteStartDocument();
                _OutputFile.WriteStartElement("gpx");
                _OutputFile.WriteAttributeString("xmlns", "http://www.topografix.com/GPX/1/1");
                _OutputFile.WriteAttributeString("creator", "ITMtoGPX");
                _OutputFile.WriteAttributeString("version", "v1.1.5");
                _OutputFile.WriteAttributeString("xmlns:xsi","http://www.w3.org/2001/XMLSchema-instance");
                _OutputFile.WriteAttributeString("xsi:schemaLocation", "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");
            }

            private void WriteBody()
            {
                //zunächst die Waypoints schreiben
                foreach(Utils.Point point in _GPX.WayPoints)
                {
                    _OutputFile.WriteStartElement("wpt");
                    _OutputFile.WriteAttributeString("lat", point.latitude.ToString(new CultureInfo("en-US")));
                    _OutputFile.WriteAttributeString("lon", point.longitude.ToString(new CultureInfo("en-US")));
                    _OutputFile.WriteElementString("ele", point.elevation.ToString(new CultureInfo("en-US")));
                    _OutputFile.WriteElementString("time", point.time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
                    _OutputFile.WriteEndElement();
                }

                //Dann die Tracks
                foreach (Utils.Track track in _GPX.Tracks)
                {
                    _OutputFile.WriteStartElement("trk");                       

                    foreach(Utils.TrackSegment tracksegment in track.TrackSegments)
                    {
                        _OutputFile.WriteStartElement("trkseg");

                        foreach (Utils.Point point in tracksegment.Points)
                        {
                            _OutputFile.WriteStartElement("trkpt");
                            _OutputFile.WriteAttributeString("lat", point.latitude.ToString(new CultureInfo("en-US")));
                            _OutputFile.WriteAttributeString("lon", point.longitude.ToString(new CultureInfo("en-US")));

                            _OutputFile.WriteElementString("ele", point.elevation.ToString(new CultureInfo("en-US")));                                    
                                _OutputFile.WriteElementString("time", point.time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
                                    
                            _OutputFile.WriteEndElement();
                        }
                        _OutputFile.WriteEndElement();
                    }

                    _OutputFile.WriteEndElement(); 
                }
            }

            private void WriteFooter()
            {
                _OutputFile.WriteEndDocument();
            }

            public void Write()
            {
                _OutputFile = new XmlTextWriter(_GPX.Filename, System.Text.Encoding.UTF8);
                _OutputFile.Formatting = Formatting.Indented;

                WriteHeader();
                WriteBody();
                WriteFooter();

                _OutputFile.Flush();
                _OutputFile.Close();
            }
        }
    }
}
