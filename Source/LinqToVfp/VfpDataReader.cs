/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System;
using System.Data.Common;

namespace LinqToVfp {
    internal class VfpDataReader : VfpClient.VfpDataReader {
        private readonly DbDataReader _dbDataReader;
        private readonly bool _autoRightTrimStrings;

        internal VfpDataReader(DbDataReader reader, bool autoRightTrimStrings)
            : base(reader) {
            if (reader == null) {
                throw new ArgumentException("reader cannot be null", "reader");
            }

            this._dbDataReader = reader;
            this._autoRightTrimStrings = autoRightTrimStrings;
        }

        public override string GetString(int i) {
            if (this._autoRightTrimStrings) {
                return base.GetString(i);
            }

            return this._dbDataReader.GetString(i);
        }
    }
}