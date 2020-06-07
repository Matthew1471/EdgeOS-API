using System.Collections.Generic;
using System.Text;

namespace EdgeOS.API
{
    class FrameReassembler
    {
        /// <summary>Contains a list of completed messages.</summary>
        readonly Stack<string> CompleteMessages = new Stack<string>();

        /// <summary>EdgeOS sometimes provides frames that are incomplete messages. This is a buffer to collect one prior to it being complete.</summary>
        readonly StringBuilder partialMessage = new StringBuilder();

        /// <summary>If the <see cref="partialMessage"/> buffer contains a partial message, we need to know the expected length of the completed message.</summary>
        uint partialMessageMissingBytes = 0;

        public void HandleReceivedData(string receivedData)
        {
            // Is there currently no partial message to complete?
            if (partialMessageMissingBytes == 0)
            {
                // Get how big this message is supposed to be (and strip the header from receivedData).
                uint expectedMessageLength = GetLengthAndStripHeader(ref receivedData);

                // Is this a complete message.
                if (receivedData.Length == expectedMessageLength)
                {
                    // There is now a complete message.
                    CompleteMessages.Push(receivedData);
                }
                // Is this an incomplete message.
                else if (receivedData.Length < expectedMessageLength)
                {
                    // Add to our partial message buffer.
                    partialMessage.Append(receivedData);
                    partialMessageMissingBytes = (uint)(expectedMessageLength - receivedData.Length);
                }
                // Do we have multiple messages.
                else if (receivedData.Length > expectedMessageLength)
                {
                    // There is now a complete message.
                    CompleteMessages.Push(receivedData.Substring(0, (int)expectedMessageLength));

                    // Add to our partial message buffer.
                    string nextMessage = receivedData.Substring((int)expectedMessageLength);
                    HandleReceivedData(nextMessage);
                }
            }
            else
            {
                // Will this complete the message exactly?
                if (receivedData.Length == partialMessageMissingBytes)
                {
                    partialMessage.Append(receivedData);
                    CompleteMessages.Push(partialMessage.ToString());

                    // There is no longer a partial message.
                    partialMessage.Clear();
                    partialMessageMissingBytes = 0;
                }
                // There still is not enough data.
                else if (receivedData.Length < partialMessageMissingBytes)
                {
                    partialMessage.Append(receivedData);
                    partialMessageMissingBytes = (uint)(partialMessageMissingBytes - receivedData.Length);
                }
                // There is enough to complete the partial message and process the next message.
                else if (receivedData.Length > partialMessageMissingBytes)
                {
                    // Add the rest of the partial message.
                    partialMessage.Append(receivedData.Substring(0, (int)partialMessageMissingBytes));

                    // Place the now completed message onto the stack.
                    CompleteMessages.Push(partialMessage.ToString());

                    // We need to remember how much more data we have as we are about to reset the partialMessageMissingBytes count.
                    uint outstandingBytes = partialMessageMissingBytes;

                    // There is no longer a partial message.
                    partialMessage.Clear();
                    partialMessageMissingBytes = 0;

                    // There is some data for the next message that needs reprocessing.
                    HandleReceivedData(receivedData.Substring((int)outstandingBytes));
                }
            }
        }

        /// <summary>Whether the FrameReassembler is expecting more characters than it has currently received.</summary>
        /// <returns>True if expecting more data, false if not.</returns>
        public bool IsMissingData() { return partialMessageMissingBytes != 0; }

        /// <summary>Whether the FrameReassembler now has a complete message frame ready for processing.</summary>
        /// <returns>True if there are complete messages awaiting processing, false if not.</returns>
        public bool HasCompleteMessages() { return CompleteMessages.Count > 0; }

        /// <summary>Gets the next complete message that this instance had been storing and then removes it from the <see cref="Stack{T}"/>.</summary>
        /// <returns>A complete message frame with no header.</returns>
        public string GetNextCompleteMessage() { return CompleteMessages.Pop(); }


        /// <summary>Reads a frame and separates the length of the message from the message itself. Returning the length to the caller and assigning back to the variable the now headerless message.</summary>
        /// <param name="messageFrame">The message that was received including the header.</param>
        /// <returns>The expected message length of this message.</returns>
        private uint GetLengthAndStripHeader(ref string messageFrame)
        {
            // The header is delimited by a new line character.
            int startMessageIndex = messageFrame.IndexOf('\n') + 1;

            // Read the frame header's expected message length.
            uint expectedMessageLength = uint.Parse(messageFrame.Substring(0, startMessageIndex));

            // Overwrite the string with the new stripped message.
            messageFrame = messageFrame.Substring(startMessageIndex);
            
            // The expected message length as per the header can now be returned.
            return expectedMessageLength;
        }
    }
}