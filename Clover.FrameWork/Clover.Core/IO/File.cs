/*
 * Copyright 2010 www.wojilu.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Clover.Core.IO
{

    public class File {

        public static String Read( String absolutePath ) {
            return Read( absolutePath, Encoding.UTF8 );
        }

        public static String Read( String absolutePath, Encoding encoding ) {
            using (StreamReader reader = new StreamReader( absolutePath, encoding )) {
                return reader.ReadToEnd();
            }
        }

        public static String[] ReadAllLines( String absolutePath ) {
            return ReadAllLines( absolutePath, Encoding.UTF8 );
        }

        public static String[] ReadAllLines( String absolutePath, Encoding encoding ) {
            ArrayList list = new ArrayList();
            using (StreamReader reader = new StreamReader( absolutePath, encoding )) {
                String str;
                while ((str = reader.ReadLine()) != null) {
                    list.Add( str );
                }
            }
            return (String[])list.ToArray( typeof( String ) );
        }

        public static void Write( String absolutePath, String fileContent ) {
            Write( absolutePath, fileContent, Encoding.UTF8 );
        }

        public static void Write( String absolutePath, String fileContent, Encoding encoding ) {
            using (StreamWriter writer = new StreamWriter( absolutePath, false, encoding )) {
                writer.Write( fileContent );
            }
        }

        public static void Delete( String absolutePath ) {
            System.IO.File.Delete( absolutePath );
        }

        public static Boolean Exists( String absolutePath ) {
            return System.IO.File.Exists( absolutePath );
        }

        public static void Move( String sourceFileName, String destFileName ) {
            System.IO.File.Move( sourceFileName, destFileName );
        }


        public static void Copy( String sourceFileName, String destFileName ) {
            System.IO.File.Copy( sourceFileName, destFileName, false );
        }

        public static void Copy( String sourceFileName, String destFileName, Boolean overwrite ) {
            System.IO.File.Copy( sourceFileName, destFileName, overwrite );
        }

        public static void Append( String absolutePath, String fileContent ) {
            Append( absolutePath, fileContent, Encoding.UTF8 );
        }

        public static void Append( String absolutePath, String fileContent, Encoding encoding ) {
            using (StreamWriter writer = new StreamWriter( absolutePath, true, encoding )) {
                writer.Write( fileContent );
            }
        }

        //public static void Zip( String sourceFileName ) {
        //    Zip( sourceFileName, sourceFileName + ".zip" );
        //}

        //public static void Zip( String sourceFileName, String destFileName ) {
        //    throw new Exception( "Zip 方法未实现" );
        //}

        //public static void UnZip( String sourceFileName ) {
        //    throw new Exception( "UnZip 方法未实现" );
        //}

        //public static void UnZip( String sourceFileName, String destFilePath ) {
        //    throw new Exception( "UnZip 方法未实现" );
        //}

    }
}

