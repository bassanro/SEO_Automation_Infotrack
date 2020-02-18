import * as React from "react";
import { Button, Modal } from "react-bootstrap";

type IModalProps = {
  showText: String | null;
  show: boolean;
  modalClosed: (event: React.MouseEvent<HTMLButtonElement>) => void;
  modalHide: () => void;
};

class ModalApp extends React.Component<IModalProps> {
  shouldComponentUpdate(nextProps: any, nextState: any) {
    return (
      nextProps.show !== this.props.show ||
      nextProps.children !== this.props.children
    );
  }

  render() {
    return (
      <React.Fragment>
        <Modal
          show={this.props.show ? true : false}
          onHide={this.props.modalHide}
        >
          <Modal.Header closeButton>
            <Modal.Title>ERROR OCCURED</Modal.Title>
          </Modal.Header>
          <Modal.Body>{this.props.showText}</Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={this.props.modalClosed}>
              Close
            </Button>
          </Modal.Footer>
        </Modal>
      </React.Fragment>
    );
  }
}

export default ModalApp;
