import java.nio.ByteBuffer;

public class IPAddressInt implements IPAddress {

	private int address;

	IPAddressInt(int address) {
		this.address = address;
	}

	@Override
	public String toString() {
		return String.format("%d.%d.%d.%d", getOctet(0), getOctet(1), getOctet(2), getOctet(3));
	}

	@Override
	public boolean equals(IPAddress other) {
		return this.toString().equals(other.toString());
	}

	@Override
	public int getOctet(int index) {
		return (int) (generateByteBuffer().get(index) & 0xFF);
	}

	@Override
	public boolean isPrivateNetwork(){

		// 10.0.0.0 - 10.255.255.255
		if (getOctet(0) == 10) {
			return true;
		}

		// 172.16.0.0 - 172.31.255.255
		if (getOctet(0) == 172 && getOctet(1) >= 16 && getOctet(1) <= 31) {
			return true;
		}

		// 192.168.0.0 - 192.168.255.255
		if (getOctet(0) == 192 && getOctet(1) == 168) {
			return true;
		}

		// 169.254.0.0 - 169.254.255.255
		if (getOctet(0) == 169 && getOctet(1) == 254) {
			return true;
		}

		return false;
	}

	private ByteBuffer generateByteBuffer() {
		ByteBuffer bb = ByteBuffer.allocate(4);
		bb.putInt(this.address);
		return bb;
	}

}
